using System.Text.Json;

namespace ResumeMatcher.Api.Infrastructure.AI;

public sealed class LlmScorer : ILlmScorer
{
    private readonly ILLMClient _llm;

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public LlmScorer(ILLMClient llm)
    {
        _llm = llm;
    }

    public async Task<IReadOnlyList<VectorMatch>> ScoreAsync(
        string jobText,
        IReadOnlyList<VectorMatch> matches,
        CancellationToken ct = default)
    {
        if (matches.Count == 0) return matches;

        var items = matches
            .Select((m, i) => new ScoringItem(
                Index: i,
                JobChunk: TrimForTokens(m.JobChunk, 1200),
                ResumeChunk: TrimForTokens(m.ResumeChunk, 1200)
            ))
            .ToList();

        var prompt = BuildPrompt(jobText, items);

        var raw = await _llm.GenerateAsync(prompt, ct);

        var response = ParseResponse(raw);

        if (response.Items.Count == 0)
            return matches;

        var updated = matches.ToList();

        foreach (var it in response.Items)
        {
            if (it.Index < 0 || it.Index >= updated.Count) continue;

            var m = updated[it.Index];

            // Ensure sane ranges + non-nulls
            m.Score = Clamp(it.Score, 0, 100);
            m.Explanation = string.IsNullOrWhiteSpace(it.Explanation) ? null : it.Explanation.Trim();
            m.MatchedSkills = it.MatchedSkills ?? new List<string>();
            m.MissingSkills = it.MissingSkills ?? new List<string>();
        }

        return updated
            .OrderByDescending(x => x.Score ?? double.MinValue)
            .ThenByDescending(x => x.Similarity)
            .ToList();
    }

    public async Task<string?> SummarizeAsync(
        string jobText,
        IReadOnlyList<VectorMatch> topMatches,
        CancellationToken ct = default)
    {
        if (topMatches.Count == 0) return null;

        var payload = new
        {
            job = TrimForTokens(jobText, 1800),
            matches = topMatches.Take(5).Select(m => new
            {
                score = m.Score,
                matchedSkills = m.MatchedSkills ?? new List<string>(),
                missingSkills = m.MissingSkills ?? new List<string>(),
                explanation = m.Explanation ?? ""
            }).ToList()
        };

        var prompt = $$"""
Return ONLY raw JSON. No markdown. No code fences. No extra text.
You MUST output valid JSON in this exact shape:
{ "summary": "max 3 sentences: overall fit (strengths), concrete evidence, and top 1-2 gaps" }

Rules:
- Use ONLY the evidence provided in matches (do not generalize).
- If Docker appears as 'familiarity', say 'basic' or 'familiarity' (not 'strong').
- Mention Azure only if it appears in missingSkills (i.e., explicitly missing).
- Keep it concise.

Input:
{{JsonSerializer.Serialize(payload, JsonOpts)}}
""";

        var raw = await _llm.GenerateAsync(prompt, ct);

        // Try parse raw
        try
        {
            using var doc = JsonDocument.Parse(raw);
            if (doc.RootElement.TryGetProperty("summary", out var s) && s.ValueKind == JsonValueKind.String)
                return s.GetString();
        }
        catch { }

        // Fallback: extract JSON object from mixed output
        var json = ExtractFirstJsonObject(raw);
        if (json is not null)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("summary", out var s) && s.ValueKind == JsonValueKind.String)
                    return s.GetString();
            }
            catch { }
        }

        return null;
    }

    private static string BuildPrompt(string jobText, List<ScoringItem> items)
    {
        var schema = """
Return ONLY raw JSON. No markdown. No code fences. No extra text.

You MUST output valid JSON in this exact shape:
{
  "items": [
    {
      "index": 0,
      "score": 0,
      "explanation": "",
      "matchedSkills": [],
      "missingSkills": []
    }
  ]
}

Rules:
- "score" MUST be an integer between 0 and 100 (0=not a match, 100=excellent match).
- Include ALL indices provided (one output object per input item).
- Use ONLY evidence from ResumeChunk; do not invent experience.
- Treat synonyms as match (e.g., "REST APIs" == "REST API development"; "CI/CD concepts" == "CI/CD").
- matchedSkills: skills/tools explicitly present in BOTH job snippet and resume snippet.
- missingSkills: important skills/tools in job snippet NOT present in resume snippet.
- Keep lists short (0-6 items each).
- Never return null for any field. Use [] or "" or 0 instead.
""";

        var payload = new
        {
            job = TrimForTokens(jobText, 2500),
            items
        };

        return $"""
You are a strict resume-to-job snippet scoring engine.

{schema}

Input:
{JsonSerializer.Serialize(payload, JsonOpts)}
""";
    }

    private static ScoringResponse ParseResponse(string raw)
    {
        // 1) Try strict JSON parse
        if (TryParse(raw, out var parsed) && parsed.Items.Count > 0)
            return Normalize(parsed);

        // 2) Try extract-first-object and parse
        var json = ExtractFirstJsonObject(raw);
        if (json is not null && TryParse(json, out parsed) && parsed.Items.Count > 0)
            return Normalize(parsed);

        return new ScoringResponse(new List<ScoredItem>());
    }

    private static bool TryParse(string json, out ScoringResponse parsed)
    {
        parsed = new ScoringResponse(new List<ScoredItem>());
        try
        {
            var tmp = JsonSerializer.Deserialize<ScoringResponse>(json, JsonOpts);
            if (tmp is null) return false;
            parsed = tmp;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static ScoringResponse Normalize(ScoringResponse r)
    {
        // Ensure defaults (avoid null lists/explanation)
        var normalized = r.Items.Select(i => i with
        {
            Explanation = i.Explanation ?? "",
            MatchedSkills = i.MatchedSkills ?? new List<string>(),
            MissingSkills = i.MissingSkills ?? new List<string>(),
            Score = Clamp(i.Score, 0, 100)
        }).ToList();

        return new ScoringResponse(normalized);
    }

    private static string? ExtractFirstJsonObject(string s)
    {
        var start = s.IndexOf('{');
        if (start < 0) return null;

        var depth = 0;
        for (int i = start; i < s.Length; i++)
        {
            if (s[i] == '{') depth++;
            else if (s[i] == '}')
            {
                depth--;
                if (depth == 0)
                    return s.Substring(start, i - start + 1);
            }
        }

        return null;
    }

    private static string TrimForTokens(string? s, int maxChars)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        s = s.Trim();
        return s.Length <= maxChars ? s : s.Substring(0, maxChars);
    }

    private static double Clamp(double v, double min, double max)
        => v < min ? min : (v > max ? max : v);

    private sealed record ScoringItem(int Index, string JobChunk, string ResumeChunk);

    // NOTE: keep Score as double in case model returns decimals; you clamp anyway.
    private sealed record ScoredItem(
        int Index,
        double Score,
        string? Explanation,
        List<string>? MatchedSkills,
        List<string>? MissingSkills);

    private sealed record ScoringResponse(List<ScoredItem> Items);
}
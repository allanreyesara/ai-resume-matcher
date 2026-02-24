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
            m.Score = it.Score;
            m.Explanation = it.Explanation;
            m.MatchedSkills = it.MatchedSkills;
            m.MissingSkills = it.MissingSkills;
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
Return ONLY valid JSON:
    { "summary": "max 3 sentences: overall fit (strengths), concrete evidence, and top 1-2 gaps" }

    Rules:
        - Use ONLY the evidence provided in matches (do not generalize).
        - If Docker appears as 'familiarity', say 'basic' or 'familiarity' (not 'strong').
        - Mention Azure only if explicitly missing.
        - Keep it concise.

Input:
{{JsonSerializer.Serialize(payload, JsonOpts)}}
""";

        var raw = await _llm.GenerateAsync(prompt, ct);

        try
        {
            using var doc = JsonDocument.Parse(raw);
            if (doc.RootElement.TryGetProperty("summary", out var s) && s.ValueKind == JsonValueKind.String)
                return s.GetString();
        }
        catch { }

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
Return ONLY valid JSON in this exact shape:
{
  "items": [
    {
      "index": 0,
      "score": 0-100,
      "explanation": "1-2 sentences, evidence-based",
      "matchedSkills": ["..."],
      "missingSkills": ["..."]
    }
  ]
}
Rules:
- Use ONLY evidence from ResumeChunk; do not invent experience.
- Treat synonyms as match (e.g., "REST APIs" == "REST API development"; "CI/CD concepts" == "CI/CD").
- matchedSkills: skills/tools explicitly present in BOTH job and resume snippet.
- missingSkills: important skills/tools in job NOT present in resume snippet.
- Keep lists short (0-6 items).
- Include ALL indices provided.
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
        try
        {
            var parsed = JsonSerializer.Deserialize<ScoringResponse>(raw, JsonOpts);
            if (parsed?.Items?.Count > 0) return parsed;
        }
        catch { }

        var json = ExtractFirstJsonObject(raw);
        if (json is not null)
        {
            var parsed = JsonSerializer.Deserialize<ScoringResponse>(json, JsonOpts);
            if (parsed?.Items?.Count > 0) return parsed;
        }

        return new ScoringResponse(new List<ScoredItem>());
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

    private sealed record ScoringItem(int Index, string JobChunk, string ResumeChunk);

    private sealed record ScoredItem(
        int Index,
        double Score,
        string? Explanation,
        List<string>? MatchedSkills,
        List<string>? MissingSkills);

    private sealed record ScoringResponse(List<ScoredItem> Items);
}
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ResumeMatcher.Api.Infrastructure.AI;

namespace ResumeMatcher.Api.Contracts.Documents;

public sealed class ResumeParserServiceImplementation : IResumeParserService
{
    private readonly ILLMClient _llmClient;
    private readonly ILogger<ResumeParserServiceImplementation> _logger;

    public ResumeParserServiceImplementation(
        ILLMClient llmClient,
        ILogger<ResumeParserServiceImplementation> logger)
    {
        _llmClient = llmClient;
        _logger = logger;
    }

    public async Task<ParsedResume> ParseResumeAsync(string normalizedText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(normalizedText))
        {
            return new ParsedResume
            {
                Meta = new ParseMeta
                {
                    Confidence = 0.0,
                    Warnings = new() { "Input text was empty or whitespace." },
                    InputChars = 0
                }
            };
        }

        var prompt = BuildPrompt(normalizedText);
        var raw = await _llmClient.GenerateAsync(prompt, cancellationToken);

        if (TryDeserialize(raw, normalizedText.Length, out var parsedResume, out var error))
            return parsedResume;

        _logger.LogWarning("Parsing failed, trying repair. Error: {Error}", error);

        var repaired = await _llmClient.GenerateAsync(BuildRepairPrompt(raw), cancellationToken);

        if (TryDeserialize(repaired, normalizedText.Length, out parsedResume, out error))
            return parsedResume;

        return new ParsedResume
        {
            Meta = new ParseMeta
            {
                Confidence = 0.1,
                Warnings = new() { "Parsing failed after repair.", error ?? "" },
                InputChars = normalizedText.Length
            }
        };
    }

    private static bool TryDeserialize(string json, int inputChars, out ParsedResume parsed, out string? error)
    {
        try
        {
            var cleaned = ExtractJsonObject(json);

            using var doc = JsonDocument.Parse(cleaned);
            var root = doc.RootElement;

            var normalizedJson = NormalizeToParsedResumeJson(root);

            parsed = JsonSerializer.Deserialize<ParsedResume>(
                normalizedJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new ParsedResume();

            var previousMeta = parsed.Meta ?? new ParseMeta();

            parsed.Meta = new ParseMeta
            {
                Confidence = previousMeta.Confidence,
                Warnings = previousMeta.Warnings ?? new List<string>(),
                Model = previousMeta.Model,
                InputChars = inputChars,
                ParsedAtUtc = DateTimeOffset.UtcNow
            };

            error = null;
            return true;
        }
        catch (Exception ex)
        {
            parsed = new ParsedResume();
            error = ex.Message;
            return false;
        }
    }

    private static string ExtractJsonObject(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";

        s = s.Trim();

        if (s.StartsWith("```", StringComparison.Ordinal))
        {
            var firstNewline = s.IndexOf('\n');
            if (firstNewline >= 0) s = s[(firstNewline + 1)..];

            var lastFence = s.LastIndexOf("```", StringComparison.Ordinal);
            if (lastFence >= 0) s = s[..lastFence];

            s = s.Trim();
        }

        var start = s.IndexOf('{');
        var end = s.LastIndexOf('}');
        if (start >= 0 && end > start)
            return s.Substring(start, end - start + 1);

        return s;
    }

    private static string NormalizeToParsedResumeJson(JsonElement root)
    {
        using var ms = new MemoryStream();
        using var writer = new Utf8JsonWriter(ms);

        writer.WriteStartObject();

        WriteIfPresent(writer, root, "candidate");
        WriteIfPresent(writer, root, "experience");
        WriteIfPresent(writer, root, "education");
        WriteStringArrayNormalized(writer, root, "certifications");
        WriteStringArrayNormalized(writer, root, "languages");
        WriteStringArrayNormalized(writer, root, "projects");
        WriteStringArrayNormalized(writer, root, "links");
        WriteIfPresent(writer, root, "meta");

        writer.WritePropertyName("skills");

        if (root.TryGetProperty("skills", out var skills))
        {
            if (skills.ValueKind == JsonValueKind.Object)
            {
                writer.WriteStartObject();

                var hard = ReadStringArrayFlexible(skills, "hardSkills", "hard", "technical", "technicalSkills", "skills");
                var soft = ReadStringArrayFlexible(skills, "softSkills", "soft", "softskills", "soft_skills");
                var tools = ReadStringArrayFlexible(skills, "tools", "tooling", "devTools", "dev_tools");

                writer.WritePropertyName("hardSkills");
                JsonSerializer.Serialize(writer, hard);

                writer.WritePropertyName("softSkills");
                JsonSerializer.Serialize(writer, soft);

                writer.WritePropertyName("tools");
                JsonSerializer.Serialize(writer, tools);

                writer.WriteEndObject();
            }
            else
            {
                var list = new List<string>();

                if (skills.ValueKind == JsonValueKind.String)
                {
                    var s = skills.GetString() ?? "";
                    list.AddRange(
                        s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    );
                }
                else if (skills.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in skills.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                            list.Add(item.GetString() ?? "");
                    }
                }

                writer.WriteStartObject();
                writer.WritePropertyName("hardSkills");
                JsonSerializer.Serialize(writer, list.Distinct().ToList());
                writer.WritePropertyName("softSkills");
                writer.WriteStartArray(); writer.WriteEndArray();
                writer.WritePropertyName("tools");
                writer.WriteStartArray(); writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }
        else
        {
            writer.WriteStartObject();
            writer.WritePropertyName("hardSkills"); writer.WriteStartArray(); writer.WriteEndArray();
            writer.WritePropertyName("softSkills"); writer.WriteStartArray(); writer.WriteEndArray();
            writer.WritePropertyName("tools"); writer.WriteStartArray(); writer.WriteEndArray();
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
        writer.Flush();

        return Encoding.UTF8.GetString(ms.ToArray());
    }

    private static void WriteStringArrayNormalized(Utf8JsonWriter writer, JsonElement root, string propName)
    {
        writer.WritePropertyName(propName);

        if (!root.TryGetProperty(propName, out var value) || value.ValueKind == JsonValueKind.Null)
        {
            writer.WriteStartArray();
            writer.WriteEndArray();
            return;
        }

        if (value.ValueKind == JsonValueKind.Array)
        {
            var list = new List<string>();

            foreach (var item in value.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    var s = item.GetString();
                    if (!string.IsNullOrWhiteSpace(s)) list.Add(s.Trim());
                    continue;
                }

                if (item.ValueKind == JsonValueKind.Object)
                {
                    var extracted =
                        TryGetString(item, "name") ??
                        TryGetString(item, "title") ??
                        TryGetString(item, "project") ??
                        TryGetString(item, "url") ??
                        TryGetString(item, "link");

                    if (!string.IsNullOrWhiteSpace(extracted))
                        list.Add(extracted.Trim());
                }
            }

            JsonSerializer.Serialize(writer, list.Distinct(StringComparer.OrdinalIgnoreCase).ToList());
            return;
        }

        if (value.ValueKind == JsonValueKind.String)
        {
            var s = value.GetString() ?? "";
            var parts = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
            JsonSerializer.Serialize(writer, parts);
            return;
        }

        writer.WriteStartArray();
        writer.WriteEndArray();
    }

private static string? TryGetString(JsonElement obj, string prop)
{
    if (obj.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String)
        return v.GetString();
    return null;
}

    private static List<string> ReadStringArrayFlexible(JsonElement obj, params string[] keys)
    {
        foreach (var k in keys)
        {
            if (!obj.TryGetProperty(k, out var v)) continue;

            if (v.ValueKind == JsonValueKind.Array)
            {
                var list = new List<string>();
                foreach (var item in v.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String)
                    {
                        var s = item.GetString();
                        if (!string.IsNullOrWhiteSpace(s)) list.Add(s.Trim());
                    }
                }
                return list.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            }

            if (v.ValueKind == JsonValueKind.String)
            {
                var s = v.GetString() ?? "";
                return s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
            }
        }

        return new List<string>();
    }

    private static void WriteIfPresent(Utf8JsonWriter writer, JsonElement root, string propName)
    {
        if (root.TryGetProperty(propName, out var value))
        {
            writer.WritePropertyName(propName);
            value.WriteTo(writer);
        }
    }

    private static string BuildPrompt(string normalizedText)
    {
        var header = """
    You are an information extraction engine.

    You MUST return a single valid JSON object and nothing else.
    - Do NOT wrap in ``` fences.
    - Do NOT include markdown or commentary.
    - The first character of your response MUST be { and the last character MUST be }.
    - Unknown values must be null or [].
    - Do not hallucinate companies, degrees, dates, or technologies.

    JSON SHAPE REQUIREMENT:
    - skills MUST be an object with exactly these keys:
    "skills": { "hardSkills": [], "softSkills": [], "tools": [] }

    Return JSON matching this shape:

    {
    "candidate": {
        "fullName": null,
        "headline": null,
        "email": null,
        "phone": null,
        "location": null
    },
    "experience": [],
    "education": [],
    "skills": {
        "hardSkills": [],
        "softSkills": [],
        "tools": []
    },
    "certifications": [],
    "languages": [],
    "projects": [],
    "links": [],
    "meta": {
        "confidence": 0.0,
        "warnings": [],
        "model": null,
        "inputChars": 0,
        "parsedAtUtc": "1970-01-01T00:00:00Z"
    }
    }

    Resume text:
    """;

        return header + "\"\"\"" + normalizedText + "\"\"\"";
    }

    private static string BuildRepairPrompt(string badOutput)
    {
        var header = """
    Return ONLY valid JSON (no markdown, no fences, no commentary).
    The first character must be { and the last character must be }.

    Fix this output:

    """;

        return header + "\"\"\"" + badOutput + "\"\"\"";
    }
}
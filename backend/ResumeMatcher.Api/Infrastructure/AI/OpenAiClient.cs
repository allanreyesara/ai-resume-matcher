using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ResumeMatcher.Api.Infrastructure.AI;

public sealed class OpenAiClient : ILLMClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public OpenAiClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<string> GenerateAsync(string prompt, CancellationToken ct = default)
    {
        var apiKey =
            _config["OpenAI:ApiKey"] ??
            _config["OPENAI_API_KEY"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Missing OpenAI:ApiKey configuration.");

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var body = new
        {
            model = "gpt-4.1-mini",
            messages = new object[]
            {
                new
                {
                    role = "system",
                    content = "You are a strict JSON generator. Output ONLY valid raw JSON. No markdown. No code fences. No extra text."
                },
                new { role = "user", content = prompt }
            },
            response_format = new { type = "json_object" }, 
            temperature = 0.1,
            max_tokens = 1200 
        };

        req.Content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
        );

        using var res = await _http.SendAsync(req, ct);
        var payload = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException($"OpenAI error ({(int)res.StatusCode} {res.ReasonPhrase}): {payload}");

        using var doc = JsonDocument.Parse(payload);

        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return content ?? "";
    }
}
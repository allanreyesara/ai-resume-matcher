using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ResumeMatcher.Api.Infrastructure.AI;

public sealed class OpenAiEmbeddingsClient : IEmbeddingsClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public OpenAiEmbeddingsClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<float[]> CreateEmbeddingAsync(string input, CancellationToken ct = default)
    {
        var apiKey = _config["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Missing OpenAI:ApiKey configuration.");

        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/embeddings");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var body = new
        {
            model = "text-embedding-3-small",
            input = input
        };

        req.Content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
        );

        using var res = await _http.SendAsync(req, ct);
        var payload = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException($"OpenAI embeddings error ({(int)res.StatusCode} {res.ReasonPhrase}): {payload}");

        using var doc = JsonDocument.Parse(payload);

        var embArray = doc.RootElement
            .GetProperty("data")[0]
            .GetProperty("embedding");

        var result = new float[embArray.GetArrayLength()];
        var i = 0;
        foreach (var n in embArray.EnumerateArray())
            result[i++] = n.GetSingle();

        return result;
    }
}
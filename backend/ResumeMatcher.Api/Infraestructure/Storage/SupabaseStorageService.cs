using System.Text.Json;
using Microsoft.Extensions.Options;

namespace ResumeMatcher.Api.Infrastructure.Storage;

public class SupabaseStorageService : IStorageService
{
    private readonly HttpClient _httpClient;
    private readonly SupabaseOptions _options;
    
    public SupabaseStorageService(HttpClient httpClient, IOptions<SupabaseOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> CreateSignedUploadUrlAsync(string bucket, string path, int expiresInSeconds = 120)
    {
        var url = $"{_options.Url}/storage/v1/object/sign/{bucket}/{path}";

        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Headers.Add("Authorization", $"Bearer {_options.ServiceRoleKey}");
        req.Headers.Add("apikey", _options.ServiceRoleKey);
        req.Content = JsonContent.Create(new { expiresIn = expiresInSeconds });

        var res = await _httpClient.SendAsync(req);
        var body = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Supabase sign failed: {(int)res.StatusCode} {res.ReasonPhrase}. Body: {body}  ");
        }
        using var json = JsonDocument.Parse(body);

        var signedUrl = json.RootElement.GetProperty("signedURL").GetString() ?? throw new InvalidOperationException("Supabase response missing 'signedURL'.");

        return signedUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? signedUrl
            : $"{_options.Url}{signedUrl}";
    }
}
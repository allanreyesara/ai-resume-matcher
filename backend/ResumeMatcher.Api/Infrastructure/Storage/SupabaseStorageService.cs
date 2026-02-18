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
        var safePath = string.Join('/', path.Split('/').Select(Uri.EscapeDataString));
        var url = $"{_options.Url}/storage/v1/object/upload/sign/{bucket}/{safePath}";

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

        string? signedUrl =
        json.RootElement.TryGetProperty("signedURL", out var p1) ? p1.GetString() :
        json.RootElement.TryGetProperty("signedUrl", out var p2) ? p2.GetString() :
        json.RootElement.TryGetProperty("url", out var p3) ? p3.GetString() :
        null;

        if (string.IsNullOrWhiteSpace(signedUrl))
            throw new InvalidOperationException($"Supabase response missing signed url. Body: {body}");

                var baseUrl = _options.Url.TrimEnd('/');

            if (signedUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return signedUrl;

            signedUrl = signedUrl.StartsWith("/") ? signedUrl : "/" + signedUrl;

            if (signedUrl.StartsWith("/object/", StringComparison.OrdinalIgnoreCase))
                return $"{baseUrl}/storage/v1{signedUrl}";

            if (signedUrl.StartsWith("/storage/v1/", StringComparison.OrdinalIgnoreCase))
                return $"{baseUrl}{signedUrl}";

            return $"{baseUrl}{signedUrl}";
    }

    public async Task<string> CreateSignedDownloadUrlAsync(string bucket, string path, int expiresInSeconds)
    {
        var safePath = string.Join('/', path.Split('/').Select(Uri.EscapeDataString));
        var url = $"{_options.Url}/storage/v1/object/sign/{bucket}/{safePath}";

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

        string? signedUrl =
            json.RootElement.TryGetProperty("signedURL", out var p1) ? p1.GetString() :
            json.RootElement.TryGetProperty("signedUrl", out var p2) ? p2.GetString() :
            json.RootElement.TryGetProperty("url", out var p3) ? p3.GetString() :
            null;

        if (string.IsNullOrWhiteSpace(signedUrl))
            throw new InvalidOperationException($"Supabase response missing signed url. Body: {body}");

        var baseUrl = _options.Url.TrimEnd('/');

        if (signedUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return signedUrl;

        signedUrl = signedUrl.StartsWith("/") ? signedUrl : "/" + signedUrl;

        if (signedUrl.StartsWith("/object/", StringComparison.OrdinalIgnoreCase))
            return $"{baseUrl}/storage/v1{signedUrl}";

        if (signedUrl.StartsWith("/storage/v1/", StringComparison.OrdinalIgnoreCase))
            return $"{baseUrl}{signedUrl}";

        return $"{baseUrl}{signedUrl}";
    }

    public async Task DeleteObjectAsync(string bucket, string path)
    {
        var safePath = string.Join('/', path.Split('/').Select(Uri.EscapeDataString));
        var url = $"{_options.Url}/storage/v1/object/{bucket}/{safePath}";

        using var req = new HttpRequestMessage(HttpMethod.Delete, url);
        req.Headers.Add("Authorization", $"Bearer {_options.ServiceRoleKey}");
        req.Headers.Add("apikey", _options.ServiceRoleKey);

        var res = await _httpClient.SendAsync(req);
        if (res.IsSuccessStatusCode) return;

        var body = await res.Content.ReadAsStringAsync();

        if ((int)res.StatusCode == 404) return;
        if (!string.IsNullOrWhiteSpace(body))
        {
            try
            {
                using var json = JsonDocument.Parse(body);
                if (json.RootElement.TryGetProperty("statusCode", out var sc))
                {
                    var scStr = sc.GetString();
                    if (scStr == "404" ) return;
                    
                }
                if (json.RootElement.TryGetProperty("error", out var err))
                {
                    var errStr = err.GetString();
                    if (string.Equals(errStr, "NotFound", StringComparison.OrdinalIgnoreCase)) return;
                    
                }
                if (json.RootElement.TryGetProperty("message", out var msg))
                {
                    var msgStr = msg.GetString();
                    if (!string.IsNullOrWhiteSpace(msgStr) && msgStr.Contains("Object not found", StringComparison.OrdinalIgnoreCase)) return;
                    
                }
            } catch
            {
                
            }
        }

        throw new InvalidOperationException($"Supabase delete failed: {(int)res.StatusCode} {res.ReasonPhrase}. Body: {body}  ");
        
    }

    public async Task<Stream> DownloadObjectAsync(string bucket, string path)
    {
        var safePath = string.Join('/', path.Split('/').Select(Uri.EscapeDataString));
        var url = $"{_options.Url}/storage/v1/object/{bucket}/{safePath}";

        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Add("Authorization", $"Bearer {_options.ServiceRoleKey}");
        var res = await _httpClient.SendAsync(req);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Supabase download failed: {(int)res.StatusCode} {res.ReasonPhrase}. Body: {body}");
        }
        return await res.Content.ReadAsStreamAsync();
    }
}
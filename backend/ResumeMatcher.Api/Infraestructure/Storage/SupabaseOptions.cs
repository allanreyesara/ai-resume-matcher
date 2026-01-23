namespace ResumeMatcher.Api.Infrastructure.Storage;

public class SupabaseOptions
{
    public string Url { get; set; } = default!;
    public string ServiceRoleKey { get; set; } = default!;
    public string Bucket { get; set; } = default!;
}
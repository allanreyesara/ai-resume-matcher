namespace ResumeMatcher.Api.Infrastructure.Storage;

public interface IStorageService
{
    Task<string> CreateSignedUploadUrlAsync(string bucket, string path, int expiresInSeconds = 120);
}
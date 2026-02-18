namespace ResumeMatcher.Api.Infrastructure.Storage;

public interface IStorageService
{
    Task<string> CreateSignedUploadUrlAsync(string bucket, string path, int expiresInSeconds = 120);
    Task<string> CreateSignedDownloadUrlAsync(string bucket, string path, int expiresInSeconds);
    Task DeleteObjectAsync(string bucket, string path);
}
namespace ResumeMatcher.Api.Infrastructure.AI;

public interface IEmbeddingsClient
{
    Task<float[]> CreateEmbeddingAsync(string input, CancellationToken ct = default);
}
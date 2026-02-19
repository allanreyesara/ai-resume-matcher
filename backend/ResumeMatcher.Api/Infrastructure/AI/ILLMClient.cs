namespace ResumeMatcher.Api.Infrastructure.AI;

public interface ILLMClient
{
    Task<string> GenerateAsync(string prompt, CancellationToken ct = default);
}
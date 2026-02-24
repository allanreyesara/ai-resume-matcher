namespace ResumeMatcher.Api.Infrastructure.Jobs;

public interface IMatchService
{
    Task<MatchResultDto> MatchAsync(
        Guid userId,
        Guid documentId,
        string jobText,
        int topK,
        bool useLlm,
        CancellationToken ct = default);
}
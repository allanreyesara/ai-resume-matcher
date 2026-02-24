namespace ResumeMatcher.Api.Infrastructure.AI;

public interface ILlmScorer
{
    Task<IReadOnlyList<VectorMatch>> ScoreAsync(
        string jobText,
        IReadOnlyList<VectorMatch> matches,
        CancellationToken ct = default);

    Task<string?> SummarizeAsync(
        string jobText,
        IReadOnlyList<VectorMatch> topMatches,
        CancellationToken ct = default);
}
namespace ResumeMatcher.Api.Contracts.Documents;

public interface IResumeParserService
{
    Task<ParsedResume> ParseResumeAsync(string normalizedText, CancellationToken cancellationToken = default);
}
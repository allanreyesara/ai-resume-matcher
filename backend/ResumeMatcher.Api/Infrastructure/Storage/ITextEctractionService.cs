namespace ResumeMatcher.Api.Infrastructure.Storage;

public interface ITextExtractionService
{
    Task ExtractTextFromPdfAsync(Guid documentId, Guid userId);
}
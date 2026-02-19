public interface ITextExtractionService
{
    Task<string> ExtractTextFromPdfAsync(Guid documentId, Guid userId, CancellationToken ct = default);
}
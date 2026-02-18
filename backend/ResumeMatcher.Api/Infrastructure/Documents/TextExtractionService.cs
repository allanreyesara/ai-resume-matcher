using UglyToad.PdfPig;
using ResumeMatcher.Api.Infrastructure.Documents;
using ResumeMatcher.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DocumentEntity = ResumeMatcher.Api.Domain.Entities.Document;


namespace ResumeMatcher.Api.Infrastructure.Storage;


public class TextExtractionService : ITextExtractionService
{
    private readonly ApplicationDbContext _db;
    private readonly IStorageService _storage;
    private readonly IPdfTextExtractor _pdfTextExtractor;
    private readonly ITextNormalizer _textNormalizer;

    public TextExtractionService(ApplicationDbContext db, IStorageService storage, IPdfTextExtractor pdfTextExtractor, ITextNormalizer textNormalizer)
    {
        _db = db;
        _storage = storage;
        _pdfTextExtractor = pdfTextExtractor;
        _textNormalizer = textNormalizer;
    }

    public async Task ExtractTextFromPdfAsync(Guid documentId, Guid userId)
    {
        var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == documentId && d.UserId == userId);

        if (doc is null)
        {
            throw new Exception("Document not found");
        } 

        if (doc.Status != DocumentEntity.DocumentStatus.Active)
        {
            throw new Exception($"Cannot extract text from document with status {doc.Status}");
        }
            
        doc.ExtractionStatus = DocumentEntity.TextExtractionStatus.InProgress;
        doc.ExtractionErrorMessage = null;
        await _db.SaveChangesAsync();

        try
        {
            var isPdf = string.Equals(doc.MimeType, "application/pdf", StringComparison.OrdinalIgnoreCase) ||
                        doc.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

            if (!isPdf)
            {
                doc.ExtractionStatus = DocumentEntity.TextExtractionStatus.Failed;
                doc.ExtractionErrorMessage = "Unsupported file type for text extraction.";
                await _db.SaveChangesAsync();
                return;
            }

            await using var stream = await _storage.DownloadObjectAsync(doc.StorageBucket, doc.StoragePath);
            var text = _pdfTextExtractor.ExtractText(stream);
            var normalizedText = _textNormalizer.Normalize(text);

            doc.ExtractedText = text;
            doc.NormalizedExtractedText = normalizedText;
            doc.ExtractionStatus = string.IsNullOrWhiteSpace(text) ? DocumentEntity.TextExtractionStatus.Failed : 
            DocumentEntity.TextExtractionStatus.Completed;

            doc.ExtractionErrorMessage = string.IsNullOrWhiteSpace(text) ? "No text extracted from PDF." : null;
            
            await _db.SaveChangesAsync();
        } catch (Exception ex)
        {
            doc.ExtractionStatus = DocumentEntity.TextExtractionStatus.Failed;
            doc.ExtractionErrorMessage = ex.Message;
            await _db.SaveChangesAsync();
        }
    }
}
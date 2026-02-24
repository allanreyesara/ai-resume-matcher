using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ResumeMatcher.Api.Contracts.Documents;
using ResumeMatcher.Api.Infrastructure.Documents; 
using ResumeMatcher.Api.Infrastructure.Data;      
using Microsoft.Extensions.Logging;
using ResumeMatcher.Api.Infrastructure.Storage;

public sealed class DocumentProcessingService
{
    private readonly ApplicationDbContext _db;
    private readonly ITextExtractionService _extractor;
    private readonly ITextNormalizer _normalizer;
    private readonly IResumeParserService _parser;
    private readonly ILogger<DocumentProcessingService> _logger;

    public DocumentProcessingService(
        ApplicationDbContext db,
        ITextExtractionService extractor,
        ITextNormalizer normalizer,
        IResumeParserService parser,
        ILogger<DocumentProcessingService> logger)
    {
        _db = db;
        _extractor = extractor;
        _normalizer = normalizer;
        _parser = parser;
        _logger = logger;
    }

    public async Task ProcessAsync(Guid documentId, CancellationToken ct = default)
    {
        var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == documentId, ct);
        if (doc is null) throw new InvalidOperationException("Document not found.");

        var userId = doc.UserId;

        var rawText = await _extractor.ExtractTextFromPdfAsync(documentId, userId);

        var normalized = _normalizer.Normalize(rawText);

        var parsed = await _parser.ParseResumeAsync(normalized, ct);

        doc.NormalizedExtractedText = normalized;
        doc.ParsedResumeJson = JsonSerializer.Serialize(parsed);
        doc.ParsedAtUtc = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Document {DocumentId} processed OK", documentId);
    }
}
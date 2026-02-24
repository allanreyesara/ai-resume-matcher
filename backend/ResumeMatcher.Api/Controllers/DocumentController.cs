using Microsoft.AspNetCore.Mvc;
using ResumeMatcher.Api.Infrastructure.Data;
using ResumeMatcher.Api.Infrastructure.Storage;
using Microsoft.AspNetCore.Authorization;
using ResumeMatcher.Api.Contracts.Documents;
using DocumentEntity = ResumeMatcher.Api.Domain.Entities.Document;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ResumeMatcher.Api.Infrastructure.AI;

using System.Reflection.Metadata;



namespace ResumeMatcher.Api.Controllers;


[ApiController]
[Route("documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IStorageService _storage;

    private readonly ITextExtractionService _textExtractionService;

    private readonly ILLMClient _llmClient;
    private readonly IEmbeddingService _embedding; 

    private const string BucketName = "documents-ai-matcher";
    private const int SignedUrlExpirySeconds = 120;

    public DocumentsController(ApplicationDbContext db, IStorageService storage, ITextExtractionService textExtractionService, ILLMClient llmClient, IEmbeddingService embedding)
    
    {
        _textExtractionService = textExtractionService;
        _db = db;
        _storage = storage;
        _embedding = embedding;
        _llmClient = llmClient;
    }

    [HttpPost("init")]
    public async Task<ActionResult<InitDocumentResponse>> Init([FromBody] InitDocumentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OriginalFileName))
            return BadRequest("Original FileName is required.");

        if (string.IsNullOrWhiteSpace(request.MimeType))
            return BadRequest("MIME type is required.");

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt" };
        var ext = Path.GetExtension(request.OriginalFileName).ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(ext) || !allowedExtensions.Contains(ext))
            return BadRequest("Unsupported file type. Allowed types are: PDF, DOC, DOCX, TXT.");

        var userId = GetUserIdOrThrow();
        var kind = MapKind(request.Kind);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var kindSegment = kind.ToString();
        var storagePath = $"{userId}/{kindSegment}/{fileName}";

        if (request.SetAsDefault)
        {
            var others = await _db.Documents.Where(
                d => d.UserId == userId 
                && d.Kind == kind
                && d.Status == DocumentEntity.DocumentStatus.Active
                && d.IsDefault
            ).ToListAsync();

            foreach (var other in others)
                other.IsDefault = false;
        }
        var doc = new DocumentEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,

            OriginalFileName = request.OriginalFileName,
            MimeType = request.MimeType,
            Kind = kind,

            FileName = fileName,
            StorageBucket = BucketName,
            StoragePath = storagePath,

            IsDefault = request.SetAsDefault,
            Status = DocumentEntity.DocumentStatus.PendingUpload,
            UploadedAt = DateTime.UtcNow
        };

        _db.Documents.Add(doc);
        await _db.SaveChangesAsync();

        var signedUrl = await _storage.CreateSignedUploadUrlAsync(
            bucket: doc.StorageBucket,
            path: doc.StoragePath,
            expiresInSeconds: SignedUrlExpirySeconds
        );

        return Ok(new InitDocumentResponse(
            DocumentId: doc.Id,
            Bucket: doc.StorageBucket,
            Path: doc.StoragePath,
            SignedUploadUrl: signedUrl
        ));
    }

    [HttpPost("{documentId:guid}/finalize")]
    public async Task<IActionResult> Finalize(Guid documentId, [FromBody] FinalizeDocumentRequest request, DocumentProcessingService processor, CancellationToken ct, ResumeEmbeddingService embeddings)
    {
        if (request.SizeBytes <= 0)
            return BadRequest("SizeBytes must be > 0");
        
        var userId = GetUserIdOrThrow();
        var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == documentId && d.UserId == userId);

        if (doc is null)
            return NotFound();
        
        doc.SizeBytes = request.SizeBytes;
        doc.Sha256Hash = request.Sha256Hash;

        doc.UploadedAt = DateTime.UtcNow;
        doc.Status = DocumentEntity.DocumentStatus.Active;

        if (doc.IsDefault)
        {
            var others = await _db.Documents.Where(d => d.UserId == userId && d.Id != doc.Id && d.Kind == doc.Kind && d.Status == DocumentEntity.DocumentStatus.Active && d.IsDefault).ToListAsync();

            foreach (var other in others)
                other.IsDefault = false;
        }

        await _db.SaveChangesAsync(ct);

        
        await processor.ProcessAsync(doc.Id, ct);
        await _textExtractionService.ExtractTextFromPdfAsync(doc.Id, userId);

        var docAfterExtract = await _db.Documents.FirstOrDefaultAsync(d => d.Id == documentId && d.UserId == userId, ct);

        if (docAfterExtract is null)
        {
            return NotFound();
        }

        var resumeText = docAfterExtract.NormalizedExtractedText
                                        ?? docAfterExtract.ExtractedText
                                        ?? docAfterExtract.ParsedResumeJson
                                        ?? "";
        if (string.IsNullOrWhiteSpace(resumeText))
        {
            return BadRequest("No extracted/normalized/parsed text available to generate embeddings.");
        }
        var already = await _db.ResumeEmbeddings.AnyAsync(x => x.DocumentId == documentId && x.UserId == userId, ct);
        if (!already)
        {
            var count = await embeddings.GenerateAndStoreAsync(docAfterExtract.Id, userId, resumeText, ct);
            return Ok(new { documentId, embeddingsChunks = count});
        }
        return Ok(new { documentId, embeddingsChunks = 0, reused = true});
        
    }

    [HttpGet("user/user-documents")]
    public async Task<ActionResult<List<UserDocument>>> GetUserDocuments()
    {
        var userId = GetUserIdOrThrow();

        var docs = await _db.Documents
            .AsNoTracking()
            .Where(d => d.UserId == userId && d.Status == DocumentEntity.DocumentStatus.Active)
            .OrderByDescending(d => d.UploadedAt)
            .Select(d => new UserDocument(
                d.Id,
                d.OriginalFileName,
                d.Kind == DocumentEntity.DocumentKind.Resume
                    ? DocumentKindDto.Resume
                    : d.Kind == DocumentEntity.DocumentKind.CoverLetter
                        ? DocumentKindDto.CoverLetter
                        : DocumentKindDto.Other,
                d.IsDefault,
                d.UploadedAt
            ))
            .ToListAsync();

        return Ok(docs);
    }

    [HttpGet("{documentId:guid}/download-url")]
    public async Task<ActionResult<DownloadUrlResponse>> GetDownloadUrl(Guid documentId)
    {
        var userId = GetUserIdOrThrow();

        var doc = await _db.Documents
            .AsNoTracking()
            .FirstOrDefaultAsync(d =>
                d.Id == documentId &&
                d.UserId == userId &&
                d.Status == DocumentEntity.DocumentStatus.Active);

        if (doc is null)
            return NotFound();

        
        var signedUrl = await _storage.CreateSignedDownloadUrlAsync(
            bucket: doc.StorageBucket,
            path: doc.StoragePath,
            expiresInSeconds: SignedUrlExpirySeconds
        );

        return Ok(new DownloadUrlResponse(signedUrl));
    }

    [HttpPost("{documentId:guid}/set-default")]
    public async Task<IActionResult> SetDefault(Guid documentId)
    {
        var userId = GetUserIdOrThrow();

        var doc = await _db.Documents.FirstOrDefaultAsync(d =>
            d.Id == documentId &&
            d.UserId == userId &&
            d.Status == DocumentEntity.DocumentStatus.Active);

        if (doc is null)
            return NotFound();

        var others = await _db.Documents.Where(d =>
            d.UserId == userId &&
            d.Id != doc.Id &&
            d.Kind == doc.Kind &&
            d.Status == DocumentEntity.DocumentStatus.Active &&
            d.IsDefault).ToListAsync();

        foreach (var other in others)
            other.IsDefault = false;

        doc.IsDefault = true;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{documentId:guid}")]
    public async Task<IActionResult> DeleteDocument(Guid documentId)
    {
        var userId = GetUserIdOrThrow();

        var doc = await _db.Documents.FirstOrDefaultAsync(d =>
            d.Id == documentId &&
            d.UserId == userId
            );

        if (doc is null)
            return NotFound();

        await _storage.DeleteObjectAsync(doc.StorageBucket, doc.StoragePath);
        _db.Documents.Remove(doc);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id:guid}/parsed")]
    public async Task<IActionResult> GetParsed(Guid id, [FromServices] ApplicationDbContext db, CancellationToken ct)
    {
        var doc = await db.Documents.FindAsync(new object[] { id }, ct);
        if (doc is null) return NotFound();

        return Ok(new {
            id = doc.Id,
            parsedAtUtc = doc.ParsedAtUtc,
            parsed = JsonSerializer.Deserialize<object>(doc.ParsedResumeJson!)
        });
    }

    private Guid GetUserIdOrThrow()
    {
        var sub = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(sub, out var id))
        {
            throw new UnauthorizedAccessException("UserId claim not found or invalid");
        }
        return id;
    }

    private static DocumentEntity.DocumentKind MapKind(DocumentKindDto kind)
    {
        return kind switch
        {
            DocumentKindDto.Resume => DocumentEntity.DocumentKind.Resume,
            DocumentKindDto.CoverLetter => DocumentEntity.DocumentKind.CoverLetter,
            DocumentKindDto.Other => DocumentEntity.DocumentKind.Other,
            _ => DocumentEntity.DocumentKind.Other
        };
    }
}
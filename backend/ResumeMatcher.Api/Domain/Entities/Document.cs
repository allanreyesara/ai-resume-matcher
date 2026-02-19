
namespace ResumeMatcher.Api.Domain.Entities;


public class Document
{
    public string? ExtractedText { get; set; }

    public string? NormalizedExtractedText { get; set; }
    public string? ParsedResumeJson { get; set; }
    public DateTimeOffset? ParsedAtUtc { get; set; }

    public TextExtractionStatus ExtractionStatus { get; set; } = TextExtractionStatus.NotStarted;
    public string? ExtractionErrorMessage { get; set; }
    public enum TextExtractionStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2,
        Failed = 3
    }
    public enum DocumentStatus
    {
        PendingUpload = 0,
        Active = 1,
        Inactive = 2,
        Deleted = 3
    }

    public enum DocumentKind
    {
        Resume,
        CoverLetter,
        Other
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public DocumentStatus Status { get; set; } = DocumentStatus.PendingUpload;
    public DocumentKind Kind { get; set; } = DocumentKind.Resume;

    public string OriginalFileName { get; set; } = default!;
    public string MimeType { get; set; } = default!;
    public string? Sha256Hash { get; set; }

    public string StorageBucket { get; set; } = default!;
    public string StoragePath { get; set; } = default!;

    public bool IsDefault { get; set; } = false;

    public long? SizeBytes { get; set; }
    
}
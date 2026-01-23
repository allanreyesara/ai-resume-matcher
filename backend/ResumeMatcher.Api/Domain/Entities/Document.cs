
namespace ResumeMatcher.Api.Domain.Entities;


public class Document
{
    public enum DocumentStatus
    {
        Active,
        Inactive,
        Deleted
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

    public DocumentStatus Status { get; set; } = DocumentStatus.Active;
    public DocumentKind Kind { get; set; } = DocumentKind.Resume;

    public string OriginalFileName { get; set; } = default!;
    public string MimeType { get; set; } = default!;
    public string? Sha256Hash { get; set; }

    public string StorageBucket { get; set; } = default!;
    public string StoragePath { get; set; } = default!;

    public bool IsDefault { get; set; } = false;


    
}
namespace ResumeMatcher.Api.Contracts.Documents;

public enum DocumentKindDto
{
    Resume,
    CoverLetter,
    Other
}

public record InitDocumentRequest(
    string OriginalFileName,
    string MimeType,
    DocumentKindDto Kind,
    bool SetAsDefault
);

public record InitDocumentResponse(
    Guid DocumentId,
    string Bucket,
    string Path,
    string SignedUploadUrl
);


public record FinalizeDocumentRequest(
    long SizeBytes,
    string? Sha256Hash
);
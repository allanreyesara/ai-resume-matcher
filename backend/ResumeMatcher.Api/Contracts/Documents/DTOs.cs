namespace ResumeMatcher.Api.Contracts.Documents;
using System.Text.Json.Serialization;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentKindDto
{
    Resume = 0,
    CoverLetter = 1,
    Other = 2
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
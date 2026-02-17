namespace ResumeMatcher.Api.Contracts.Documents;

public record UserDocument(
    Guid Id,
    string OriginalFileName,
    DocumentKindDto Kind,
    bool IsDefault,
    DateTime UploadedAt
);
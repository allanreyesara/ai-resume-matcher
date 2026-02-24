public record MatchRequest(
    Guid UserId,
    Guid DocumentId,
    string JobText,
    int TopK,
    bool UseLlm
);
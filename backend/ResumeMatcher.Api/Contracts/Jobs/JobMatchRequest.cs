public record MatchRequest(
    string JobText,
    int TopK,
    bool UseLlm
);
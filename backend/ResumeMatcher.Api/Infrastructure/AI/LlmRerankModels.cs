namespace ResumeMatcher.Api.Infrastructure.AI;

public sealed record LlmRerankRequestItem(int Index, string Text);

public sealed record LlmRerankResponseItem(int Index, double Score, string? Reason);

public sealed record LlmRerankResponse(List<LlmRerankResponseItem> Items);
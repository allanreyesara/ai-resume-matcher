public sealed class MatchItemDto
{
    public int Rank { get; init; }
    public double Similarity { get; init; } 
    public double? Score { get; init; } 
    public string JobChunk { get; init; } = string.Empty;
    public string ResumeChunk { get; init; } = string.Empty;
    public string? Explanation { get; init; }
    public List<string>? MatchedSkills { get; init; }
    public List<string>? MissingSkills { get; init; }
}
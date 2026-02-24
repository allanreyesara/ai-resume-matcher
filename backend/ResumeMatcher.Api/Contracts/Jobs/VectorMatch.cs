public sealed class VectorMatch
{
    public int Rank { get; set; }
    public double Similarity { get; set; }
    public double? Score { get; set; }
    public string JobChunk { get; set; } = string.Empty;
    public string ResumeChunk { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public List<string>? MatchedSkills { get; set; }
    public List<string>? MissingSkills { get; set; }
}
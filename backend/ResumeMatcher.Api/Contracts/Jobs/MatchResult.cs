public sealed class MatchResultDto
{
    public Guid DocumentId { get; init; }
    public int TopK { get; init; }
    public bool UsedLlm { get; init; }

    public double OverallScore { get; init; }
    public string? Summary { get; init;}

    public List<MatchItemDto> ?Matches { get; set; }

    public MatchMetaDto Meta { get; init; } = new();
}

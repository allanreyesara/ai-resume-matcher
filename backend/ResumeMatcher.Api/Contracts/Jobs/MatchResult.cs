public sealed class MatchResultDto
{
    public Guid DocumentId { get; init; }
    public int TopK { get; init; }
    public bool UsedLlm { get; init; }

    public double OverallScorePercent { get; init; }
    public string? Summary { get; init;}

    public List<MatchItemDto> Matches { get; init; } = new();

    public MatchMetaDto Meta { get; init; } = new();
}

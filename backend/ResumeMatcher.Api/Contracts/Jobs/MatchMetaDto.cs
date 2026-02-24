public sealed class MatchMetaDto
{
    public int JobChunkCount { get; init; }
    public int ResumeChunkCount { get; init; }

    public long ProcessingTimeMs { get; init; }

    // Versionado para que luego no rompas todo
    public string PipelineVersion { get; init; } = "v1";
}
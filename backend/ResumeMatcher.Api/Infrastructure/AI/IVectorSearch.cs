namespace ResumeMatcher.Api.Infrastructure.AI;

public interface IVectorSearch
{
    List<VectorMatch> FindTopMatches(
        List<string> jobChunks,
        List<float[]> jobVectors,
        List<ResumeEmbedding> resumeVectors,
        int topK);
}
using ResumeMatcher.Api.Infrastructure.AI;

public class VectorSearch : IVectorSearch
{
    public List<VectorMatch> FindTopMatches(
        List<string> jobChunks,
        List<float[]> jobVectors,
        List<ResumeEmbedding> resumeVectors,
        int topK)
    {
        var results = new List<VectorMatch>();

        for (int i = 0; i < jobVectors.Count; i++)
        {
            var jobVec = jobVectors[i];
            var jobChunk = jobChunks[i];

            foreach (var res in resumeVectors)
            {
                var sim = Cosine(jobVec, res.Vector);

                results.Add(new VectorMatch
                {
                    JobChunk = jobChunk,
                    ResumeChunk = res.ChunkText,
                    Similarity = sim
                });
            }
        }

        return results
            .OrderByDescending(x => x.Similarity)
            .Take(topK)
            .ToList();
    }

    private static double Cosine(float[] a, float[] b)
    {
        double dot = 0, magA = 0, magB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }

        return dot / (Math.Sqrt(magA) * Math.Sqrt(magB) + 1e-10);
    }
}
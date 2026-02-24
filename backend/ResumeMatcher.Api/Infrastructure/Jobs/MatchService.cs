using System.Diagnostics;
using ResumeMatcher.Api.Infrastructure.AI;
using ResumeMatcher.Api.Infrastructure.Documents;

namespace ResumeMatcher.Api.Infrastructure.Jobs;

public class MatchService : IMatchService
{
    private readonly ITextNormalizer _textNormalizer;
    private readonly IEmbeddingService _embeddingService;
    private readonly ITextChunker _chunker;
    private readonly IResumeVectorRepository _vectorRepo;
    private readonly IVectorSearch _vectorSearch;
    private readonly ILlmScorer _llmScorer;

    public MatchService(
        ITextNormalizer textNormalizer,
        IEmbeddingService embeddingService,
        ITextChunker chunker,
        IResumeVectorRepository vectorRepo,
        IVectorSearch vectorSearch,
        ILlmScorer llmScorer)
    {
        _textNormalizer = textNormalizer;
        _embeddingService = embeddingService;
        _chunker = chunker;
        _vectorRepo = vectorRepo;
        _vectorSearch = vectorSearch;
        _llmScorer = llmScorer;
    }

    public async Task<MatchResultDto> MatchAsync(
        Guid userId,
        Guid documentId,
        string jobText,
        int topK,
        bool useLlm,
        CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();

        var normalizedJob = _textNormalizer.Normalize(jobText);
        var jobChunks = _chunker.Chunk(normalizedJob);
        var jobVectors = await _embeddingService.GenerateAsync(jobChunks);
        var resumeVectors = await _vectorRepo.GetByDocumentId(documentId, userId);
        var candidateK = useLlm ? Math.Max(topK * 4, 20) : topK;

        var similarities = _vectorSearch.FindTopMatches(
            jobChunks,
            jobVectors,
            resumeVectors,
            candidateK
        );

        if (useLlm)
        {
            similarities = (await _llmScorer.ScoreAsync(jobText, similarities, ct)).ToList();
            similarities = similarities.Take(topK).ToList();
        }
        else
        {
            similarities = similarities.Take(topK).ToList();
        }

        string? summary = null;
        if (useLlm && similarities.Count > 0)
            summary = await _llmScorer.SummarizeAsync(jobText, similarities, ct);

        for (int i = 0; i < similarities.Count; i++)
            similarities[i].Rank = i + 1;

        var items = similarities.Select(s => new MatchItemDto
        {
            Rank = s.Rank,
            ResumeChunk = s.ResumeChunk,
            JobChunk = s.JobChunk,
            Similarity = s.Similarity,
            Score = s.Score,
            Explanation = s.Explanation,
            MatchedSkills = s.MatchedSkills,
            MissingSkills = s.MissingSkills
        }).ToList();

        double overall01 = 0.0;

        if (items.Count > 0)
        {
            overall01 = items.Average(x =>
            {
                var sim = x.Similarity; 

                var llm01 = x.Score.HasValue ? (x.Score.Value / 100.0) : (double?)null;

                if (!useLlm || llm01 is null)
                    return sim; 
                const double wSim = 0.7;
                const double wLlm = 0.3;
                return (wSim * sim) + (wLlm * llm01.Value);
            });
        }
        overall01 = Math.Max(0.0, Math.Min(1.0, overall01));

        sw.Stop();

        return new MatchResultDto
        {
            DocumentId = documentId,
            TopK = topK,
            UsedLlm = useLlm,
            OverallScorePercent = overall01 * 100.0,
            Summary = summary,
            Matches = items,
            Meta = new MatchMetaDto
            {
                JobChunkCount = jobChunks.Count,
                ResumeChunkCount = resumeVectors.Count,
                ProcessingTimeMs = sw.ElapsedMilliseconds,
                PipelineVersion = "v1"
            }
        };
    }
}
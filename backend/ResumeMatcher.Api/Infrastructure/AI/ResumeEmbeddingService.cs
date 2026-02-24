using Microsoft.EntityFrameworkCore;
using ResumeMatcher.Api.Domain.Entities;
using ResumeMatcher.Api.Infrastructure.Data;

namespace ResumeMatcher.Api.Infrastructure.AI;

public sealed class ResumeEmbeddingService
{
    private readonly ApplicationDbContext _db;
    private readonly ITextChunker _chunker;
    private readonly IEmbeddingService _embeddings;

    public ResumeEmbeddingService(ApplicationDbContext db, ITextChunker chunker, IEmbeddingService embeddings)
    {
        _db = db;
        _chunker = chunker;
        _embeddings = embeddings;
    }

    public async Task<int> GenerateAndStoreAsync(
        Guid documentId,
        Guid userId,
        string resumeText,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(resumeText))
            throw new InvalidOperationException("resumeText is empty; cannot generate embeddings.");

        var chunks = _chunker.Chunk(resumeText);
        if (chunks.Count == 0)
            throw new InvalidOperationException("No chunks generated.");

        var vectors = await _embeddings.GenerateAsync(chunks);

        var existing = await _db.ResumeEmbeddings
            .Where(x => x.DocumentId == documentId && x.UserId == userId)
            .ToListAsync(ct);

        if (existing.Count > 0)
            _db.ResumeEmbeddings.RemoveRange(existing);

        for (int i = 0; i < chunks.Count; i++)
        {
            _db.ResumeEmbeddings.Add(new ResumeEmbedding
            {
                Id = Guid.NewGuid(),
                DocumentId = documentId,
                UserId = userId,
                ChunkIndex = i,
                ChunkText = chunks[i],
                Vector = vectors[i]
            });
        }

        await _db.SaveChangesAsync(ct);
        return chunks.Count;
    }
}
using Microsoft.EntityFrameworkCore;
using ResumeMatcher.Api.Infrastructure.Data;

public class ResumeVectorRepository : IResumeVectorRepository
{
    private readonly ApplicationDbContext _db;

    public ResumeVectorRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    
    public async Task<List<ResumeEmbedding>> GetByDocumentId(Guid documentId, Guid userId)
    {
        return await _db.ResumeEmbeddings
            .Where(x => x.DocumentId == documentId && x.UserId == userId)
            .ToListAsync();
    } 
}


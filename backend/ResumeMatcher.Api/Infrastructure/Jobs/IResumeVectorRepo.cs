public interface IResumeVectorRepository
{
    Task<List<ResumeEmbedding>> GetByDocumentId(Guid documentId, Guid userId);
}
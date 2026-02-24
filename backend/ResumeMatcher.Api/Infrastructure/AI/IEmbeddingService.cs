public interface IEmbeddingService
{
    Task<List<float[]>> GenerateAsync(List<string> chunks);
}
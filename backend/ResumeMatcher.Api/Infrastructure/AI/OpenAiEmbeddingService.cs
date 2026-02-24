using ResumeMatcher.Api.Infrastructure.AI;

public class OpenAiEmbeddingService : IEmbeddingService
{
    private readonly OpenAiEmbeddingsClient _client;

    public OpenAiEmbeddingService(OpenAiEmbeddingsClient client)
    {
        _client = client;
    }

    public async Task<List<float[]>> GenerateAsync(List<string> chunks)
    {
        var vectors = new List<float[]>();

        foreach (var chunk in chunks)
        {
            var embedding = await _client.CreateEmbeddingAsync(chunk);
            vectors.Add(embedding);
        }

        return vectors;
    }
}
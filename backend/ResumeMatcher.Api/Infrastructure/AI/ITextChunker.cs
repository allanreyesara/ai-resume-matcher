namespace ResumeMatcher.Api.Infrastructure.AI;

public interface ITextChunker
{
    List<string> Chunk(string text, int maxChars = 800);
}
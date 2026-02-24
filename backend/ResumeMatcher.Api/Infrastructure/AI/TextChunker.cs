using System.Text;

namespace ResumeMatcher.Api.Infrastructure.AI;

public sealed class TextChunker : ITextChunker
{
    public List<string> Chunk(string text, int maxChars = 800)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new();

        var paragraphs = text
            .Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .ToList();

        var chunks = new List<string>();
        var current = new StringBuilder();

        foreach (var para in paragraphs)
        {
            if (current.Length + para.Length + 1 <= maxChars)
            {
                if (current.Length > 0)
                    current.Append("\n");

                current.Append(para);
            }
            else
            {
                if (current.Length > 0)
                {
                    chunks.Add(current.ToString());
                    current.Clear();
                }

                if (para.Length > maxChars)
                {
                    for (int i = 0; i < para.Length; i += maxChars)
                    {
                        var slice = para.Substring(i, Math.Min(maxChars, para.Length - i));
                        chunks.Add(slice);
                    }
                }
                else
                {
                    current.Append(para);
                }
            }
        }

        if (current.Length > 0)
            chunks.Add(current.ToString());

        return chunks;
    }
}
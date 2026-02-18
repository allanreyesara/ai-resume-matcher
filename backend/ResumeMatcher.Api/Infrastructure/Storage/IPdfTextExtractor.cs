using System.Text;
using UglyToad.PdfPig;

namespace ResumeMatcher.Api.Infrastructure.Storage
{
    public interface IPdfTextExtractor
    {
        string ExtractText(Stream pdfStream);
    }

    public class PdfTextExtractor : IPdfTextExtractor
    {
        public string ExtractText(Stream pdfStream)
        {
            if (!pdfStream.CanSeek)
            {
                using var ms = new MemoryStream();
                pdfStream.CopyTo(ms);
                ms.Position = 0;
                return ExtractText(ms);
            }

            pdfStream.Position = 0;
            using var document = PdfDocument.Open(pdfStream);
            var sb = new StringBuilder();

            foreach (var page in document.GetPages())
            {
                var t = page.Text;
                if (!string.IsNullOrWhiteSpace(t))
                {
                    sb.AppendLine(t);
                }
            }
            return sb.ToString().Trim();
        }
    }
}
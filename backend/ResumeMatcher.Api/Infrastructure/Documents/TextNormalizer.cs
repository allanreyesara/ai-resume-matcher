using System.Text;
using System.Text.RegularExpressions;

namespace ResumeMatcher.Api.Infrastructure.Documents
{
    

    public class TextNormalizer : ITextNormalizer
    {
        private static readonly Regex MultipleSpaces = new(@"[ \t\t]{2,}", RegexOptions.Compiled);
        private static readonly Regex ManyBlankLines = new(@"\n{3,}", RegexOptions.Compiled);

        public string Normalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var s = text.Replace("\r\n", "\n").Replace("\r", "\n");
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                if (ch == '\u200B' || ch == '\uFEFF') continue;
                if (char.IsControl(ch) && ch != '\n' && ch != '\t') continue;
                sb.Append(ch);
            }
            s = sb.ToString();

            s = s.Replace("\t", " ");
            s = MultipleSpaces.Replace(s, " ");
            s = TrimEachLine(s);
            s = ManyBlankLines.Replace(s, "\n\n");
            s = s
                .Replace("•", "-")
                .Replace("‣", "-")
                .Replace("∙", "-")
                .Replace("◦", "-")
                .Replace("▪", "-");
            s = FixHyphenationAcrossLineBreasks(s);

            return s.Trim();
        }

        private static string TrimEachLine(string text)
        {
            var lines = text.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
            }
            return string.Join("\n", lines);
        }

        private static string FixHyphenationAcrossLineBreasks(string text)
        {
            var sb = new StringBuilder(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '-' && i +1 < text.Length && text[i + 1] == '\n')
                {
                    var prev = i - 1 >= 0 ? text[i - 1] : '\0';
                    var nextIndex = i + 2;
                    var next = nextIndex < text.Length ? text[nextIndex] : '\0';
                    if (char.IsLetter(prev) && char.IsLetter(next))
                    {
                        // If both previous and next characters are letters, remove the hyphen and newline
                        i++; // Skip the newline character
                        continue;
                    }
                }
                sb.Append(text[i]);
            }
            return sb.ToString();
        }
    }
}
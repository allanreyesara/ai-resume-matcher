namespace  ResumeMatcher.Api.Contracts.Documents;

public sealed class ParsedResume
{
    public CandidateInfo Candidate { get; init; } = new();
    public List<ExperienceItem> Experience { get; init; } = new();
    public List<EducationItem> Education { get; init; } = new();
    public SkillsSection Skills { get; init; } = new();
    public List<string> Certifications { get; init; } = new();
    public List<string> Languages { get; init; } = new();
    public List<string> Projects { get; init; } = new();
    public List<string> Links { get; init; } = new();
    public ParseMeta Meta { get; set; } = new();
}
public sealed class ParseResumeRequest
{
    public string NormalizedText { get; set; } = "";
}

public sealed class CandidateInfo
{
    public string? FullName { get; init; }
    public string? Headline { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Location { get; init; }
}

public sealed class ExperienceItem
{
    public string? Title { get; init; }
    public string? Company { get; init; }
    public string? Location { get; init; }
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
    public List<string> Highlights { get; init; } = new();
    public List<string> Technologies { get; init; } = new();
}

public sealed class EducationItem
{
    public string? Degree { get; init; }
    public string? Institution { get; init; }
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
    public string? Notes { get; init; }
}

public sealed class SkillsSection
{
    public List<string> HardSkills { get; init; } = new();
    public List<string> SoftSkills { get; init; } = new();
    public List<string> Tools { get; init; } = new();

}

public sealed class ParseMeta
{
    public double Confidence { get; init; } = 0.0;
    public List<string> Warnings { get; init; } = new();
    public string? Model { get; init; }
    public int InputChars { get; init; }
    public DateTimeOffset ParsedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
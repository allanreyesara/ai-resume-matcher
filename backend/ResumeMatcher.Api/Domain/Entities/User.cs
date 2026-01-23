namespace ResumeMatcher.Api.Domain.Entities;

public class User

{
   public Guid Id { get; set; }
   public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string FullName { get; set; } = default!;
    public bool IsActive { get; set; } = true;


    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    
}
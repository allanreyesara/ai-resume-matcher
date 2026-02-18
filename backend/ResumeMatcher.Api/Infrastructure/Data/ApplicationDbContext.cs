using Microsoft.EntityFrameworkCore;
using ResumeMatcher.Api.Domain.Entities;

namespace ResumeMatcher.Api.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Document> Documents => Set<Document>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.HashedToken).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.HashedToken).IsUnique();

            entity.Property(e => e.ExpiresAtUtc).IsRequired();
            entity.Property(e => e.CreatedAtUtc).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.HasIndex(e => e.UserId);  

            entity.HasOne(e => e.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.Property(e => e.CreatedByIp).HasMaxLength(200);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Documents");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserId).IsRequired();

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.Kind });
            entity.HasIndex(e => new { e.UserId, e.Kind, e.IsDefault });

            entity.HasOne(e => e.User).WithMany(u => u.Documents).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.UploadedAt).IsRequired();
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.OriginalFileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.MimeType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Sha256Hash).HasMaxLength(64);
            entity.Property(e => e.StorageBucket).HasMaxLength(255).IsRequired();
            entity.Property(e => e.StoragePath).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Kind).HasConversion<string>().HasMaxLength(255).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().IsRequired();
            entity.Property(e => e.IsDefault).IsRequired().HasDefaultValue(false);
        });
    }
}
using Microsoft.EntityFrameworkCore;
using HireSphere.Core.Models;

namespace HireSphere.Infrastructure.ORM;

public class HireSphereDbContext : DbContext
{
    public HireSphereDbContext(DbContextOptions<HireSphereDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
            entity.Property(rt => rt.ExpiryDate).IsRequired();
            entity.Property(rt => rt.IsRevoked).IsRequired();
            entity.Property(rt => rt.CreatedAt).IsRequired();
            entity.Property(rt => rt.UserId).IsRequired();

            entity.HasOne(rt => rt.User)
                  .WithMany()
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(rt => rt.Token).IsUnique();
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(prt => prt.Id);
            entity.Property(prt => prt.Token).IsRequired().HasMaxLength(500);
            entity.Property(prt => prt.ExpiryDate).IsRequired();
            entity.Property(prt => prt.IsUsed).IsRequired();
            entity.Property(prt => prt.CreatedAt).IsRequired();
            entity.Property(prt => prt.UserId).IsRequired();

            entity.HasOne(prt => prt.User)
                  .WithMany()
                  .HasForeignKey(prt => prt.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(prt => prt.Token).IsUnique();
        });
    }
}

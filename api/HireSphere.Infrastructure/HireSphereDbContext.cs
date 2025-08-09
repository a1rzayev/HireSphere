using Microsoft.EntityFrameworkCore;
using HireSphere.Core.Models;

namespace HireSphere.Infrastructure;

public class HireSphereDbContext : DbContext
{
    public HireSphereDbContext(DbContextOptions<HireSphereDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

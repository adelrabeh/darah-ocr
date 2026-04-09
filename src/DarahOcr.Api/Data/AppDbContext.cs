using Microsoft.EntityFrameworkCore;
using DarahOcr.Api.Models;

namespace DarahOcr.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<OcrJob> OcrJobs => Set<OcrJob>();
    public DbSet<OcrResult> OcrResults => Set<OcrResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Username).IsUnique();
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<OcrJob>(e =>
        {
            e.HasOne(j => j.User).WithMany(u => u.Jobs).HasForeignKey(j => j.UserId);
            e.HasOne(j => j.Result).WithOne(r => r.Job).HasForeignKey<OcrResult>(r => r.JobId);
        });
    }
}

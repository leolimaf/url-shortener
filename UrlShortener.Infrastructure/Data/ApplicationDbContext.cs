using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Data.Configurations;

namespace UrlShortener.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
    public DbSet<VisitedUrl> VisitedUrls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ShortenedUrlConfiguration());
        modelBuilder.ApplyConfiguration(new VisitedUrlConfiguration());
    }
}
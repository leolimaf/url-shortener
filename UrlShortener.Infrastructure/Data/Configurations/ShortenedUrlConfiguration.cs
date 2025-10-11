using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Application.Services;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Data.Configurations;

public class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
{
    public void Configure(EntityTypeBuilder<ShortenedUrl> builder)
    {
        builder.ToTable("SHORTENED_URL");
        
        builder.HasKey(su => su.Id);
        
        builder.Property(su => su.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("ID");
        
        builder.Property(su => su.Code)
            .HasColumnName("CODE")
            .HasMaxLength(UrlShortenerService.Length);
        
        builder.Property(su => su.OriginalUrl)
            .HasColumnName("ORIGINAL_URL")
            .IsRequired();
        
        builder.Property(su => su.CreatedAtUtc)
            .HasColumnName("CREATED_AT_UTC")
            .HasDefaultValueSql("GETUTCDATE()");
        
        builder.HasIndex(su => su.Code)
            .IsUnique();
    }
}
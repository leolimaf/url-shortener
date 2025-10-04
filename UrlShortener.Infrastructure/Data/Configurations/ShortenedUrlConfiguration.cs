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
        
        builder.Property(su => su.LongUrl)
            .HasColumnName("LONG_URL")
            .IsRequired();
        
        builder.Property(su => su.ShortUrl)
            .HasColumnName("SHORT_URL")
            .IsRequired();

        builder.Property(su => su.Code)
            .HasColumnName("CODE")
            .HasMaxLength(UrlShortenerService.NumberOfCharactersInShortLink);
        
        builder.Property(su => su.CreatedAtUtc)
            .HasColumnName("CREATED_AT_UTC")
            .IsRequired();
        
        builder.HasIndex(su => su.Code)
            .IsUnique();
    }
}
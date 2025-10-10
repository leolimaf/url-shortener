using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Application.Services;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Data.Configurations;

public class VisitedUrlConfiguration : IEntityTypeConfiguration<VisitedUrl>
{
    public void Configure(EntityTypeBuilder<VisitedUrl> builder)
    {
        builder.ToTable("VISITED_URL");
        
        builder.HasKey(vu => vu.Id);
        
        builder.Property(vu => vu.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("ID");
        
        builder.Property(vu => vu.Code)
            .HasColumnName("CODE")
            .HasMaxLength(UrlShortenerService.Length);
        
        builder.Property(vu => vu.VisitedAt)
            .HasColumnName("VISITED_AT")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(vu => vu.UserAgent)
            .HasColumnName("USER_AGENT");
        
        builder.Property(vu => vu.Referer)
            .HasColumnName("REFERER");
        
        builder.Property(vu => vu.IpAddress)
            .HasColumnName("IP_ADDRESS");
        
        builder.Property(vu => vu.ShortenedUrlId)
            .HasColumnName("SHORTENED_URL_ID")
            .IsRequired();

        builder.HasOne(vu => vu.ShortenedUrl)
            .WithMany(su => su.VisitedUrls)
            .HasForeignKey(vu => vu.ShortenedUrlId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(vu => vu.Code);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("USER");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("ID");
        
        builder.Property(u => u.FullName)
            .HasColumnName("FULL_NAME")
            .IsRequired();

        builder.Property(u => u.BirthDate)
            .HasColumnName("BIRTH_DATE");
        
        builder.Property(u => u.Phone)
            .HasColumnName("PHONE");
        
        builder.Property(u => u.Email)
            .HasColumnName("EMAIL")
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(u => u.PasswordHash)
            .HasColumnName("PASSWORD_HASH")
            .IsRequired();
        
        builder.Property(u => u.IsEmailConfirmed)
            .HasColumnName("IS_EMAIL_CONFIRMED")
            .HasDefaultValue(false);
        
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_USER_EMAIL_UNIQUE");
    }
}
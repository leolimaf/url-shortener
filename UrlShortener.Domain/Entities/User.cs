namespace UrlShortener.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public string? Phone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
    
    public virtual ICollection<ShortenedUrl> ShortenedUrls { get; set; } = new List<ShortenedUrl>();
}
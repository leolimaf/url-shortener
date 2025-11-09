namespace UrlShortener.Domain.Entities;

public class ShortenedUrl
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    
    public long UserId  { get; set; }
    public virtual User? User { get; set; }

    public virtual ICollection<VisitedUrl> VisitedUrls { get; set; } = new List<VisitedUrl>();
}
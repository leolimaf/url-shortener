namespace UrlShortener.Domain.Entities;

public class VisitedUrl
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime VisitedAt { get; set; }
    public string? UserAgent { get; set; }
    public string? Referer { get; set; }

    public long ShortenedUrlId  { get; set; }
    public virtual ShortenedUrl ShortenedUrl { get; set; } = new();
}
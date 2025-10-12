using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Contracts;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories;

public class UrlShortenerRepository(ApplicationDbContext context) : IUrlShortenerRepository
{
    public async Task AddShortenedUrl(ShortenedUrl shortenedUrl)
    {
        await context.ShortenedUrls.AddAsync(shortenedUrl);
    }

    public async Task AddVisitedUrl(VisitedUrl visitedUrl)
    {
        await context.Database.ExecuteSqlInterpolatedAsync($@"
           INSERT INTO VISITED_URL (CODE, VISITED_AT_UTC, USER_AGENT, REFERER, IP_ADDRESS, SHORTENED_URL_ID)
           SELECT {visitedUrl.Code}, {visitedUrl.VisitedAtUtc}, {visitedUrl.UserAgent}, {visitedUrl.Referer}, 
                  {visitedUrl.IpAddress}, Id
           FROM SHORTENED_URL
           WHERE CODE = {visitedUrl.Code};
        ");
    }

    public async Task<string?> GetOriginalUrl(string code, CancellationToken token)
    {
        return await context.ShortenedUrls
            .AsNoTracking()
            .Where(x => x.Code == code)
            .Select(x => x.OriginalUrl)
            .FirstOrDefaultAsync(cancellationToken: token);
    }
}
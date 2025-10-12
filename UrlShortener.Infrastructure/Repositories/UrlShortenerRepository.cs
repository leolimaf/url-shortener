using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Contracts;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories;

public class UrlShortenerRepository(ApplicationDbContext context) : IUrlShortenerRepository
{
    public async Task Add(ShortenedUrl shortenedUrl)
    {
        if (shortenedUrl.Id <= 0)
            await context.ShortenedUrls.AddAsync(shortenedUrl);
        else
        {
            context.ShortenedUrls.Attach(shortenedUrl);
            context.Entry(shortenedUrl).State = EntityState.Modified;
        }
    }

    public async Task<ShortenedUrl?> Get(string code)
    {
        return await context.ShortenedUrls
            .Include(x => x.VisitedUrls)
            .FirstOrDefaultAsync(x => x.Code == code);
    }
}
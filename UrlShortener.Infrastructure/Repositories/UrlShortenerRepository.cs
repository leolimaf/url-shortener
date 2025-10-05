using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories;

public class UrlShortenerRepository(ApplicationDbContext context) : IUrlShortenerRepository
{
    public async Task<bool> Exists(string code)
    {
        return await context.ShortenedUrls.AnyAsync(x => x.Code == code);
    }

    public async Task Add(ShortenedUrl shortenedUrl)
    {
        await context.AddAsync(shortenedUrl);
    }

    public async Task<ShortenedUrl?> Get(string code)
    {
        return await context.ShortenedUrls.FirstOrDefaultAsync(x => x.Code == code);
    }
}
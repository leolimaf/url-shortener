using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Repositories;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories;

public class UrlShortenerRepository(ApplicationDbContext context) : IUrlShortenerRepository
{
    public async Task<bool> Exists(string code)
    {
        return await context.ShortenedUrls.AnyAsync(x => x.Code == code);
    }
}
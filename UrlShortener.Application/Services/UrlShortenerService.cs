using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs.Requests;
using UrlShortener.Application.DTOs.Response;
using UrlShortener.Domain.Contracts;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Services;

public class UrlShortenerService(
    ILogger<UrlShortenerService> logger,
    IUrlShortenerRepository urlShortenerRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext
    ) : IUrlShortenerService
{
    public const int Length = 7;
    
    public async Task<ShortenUrlResponse> IncludeShortenedUrl(ShortenUrlRequest request)
    {
        const int maxAttempts = 5;
        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var code = GenerateCode();
            try
            {
                var shortenedUrl = new ShortenedUrl
                {
                    OriginalUrl = request.Url,
                    Code = code,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await urlShortenerRepository.AddShortenedUrl(shortenedUrl);
                await unitOfWork.SaveAsync();

                return new ShortenUrlResponse(shortenedUrl.Code);
            }
            catch (DbUpdateException e)
            {
                if (attempt + 1 == maxAttempts)
                {
                    logger.LogError(e, "Failed to generate a unique code after {0} attempts.", maxAttempts);
                    throw;
                }

                logger.LogWarning("Code collision detected. Retrying... Attempt {0} of {1}", attempt + 1, maxAttempts);
            }
        }
        throw new InvalidOperationException("Failed to generate a unique code.");
    }
    
    private static string GenerateCode()
    {
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        var chars = Enumerable.Range(0, Length)
            .Select(_ => characters[Random.Shared.Next(characters.Length)])
            .ToArray();

        return new string(chars);
    }

    public async Task<GetOriginalUrlResponse> GetOriginalUrl(string code)
    {
        var originalUrl = await urlShortenerRepository.GetOriginalUrl(code);

        if (!string.IsNullOrEmpty(originalUrl))
        {
            var visitedUrl = new VisitedUrl
            {
                Code = code,
                VisitedAtUtc = DateTime.UtcNow,
                UserAgent = userContext.UserAgent,
                Referer = userContext.Referer,
                IpAddress = userContext.IpAddress
            };
            
            await urlShortenerRepository.AddVisitedUrl(visitedUrl);
        }

        return new GetOriginalUrlResponse(originalUrl);
    }
}
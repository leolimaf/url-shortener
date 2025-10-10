using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs.Requests;
using UrlShortener.Application.DTOs.Response;
using UrlShortener.Domain.Contracts;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Services;

public class UrlShortenerService(
    IUrlShortenerRepository urlShortenerRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext
    ) : IUrlShortenerService
{
    public const int Length = 7;
    
    public async Task<ShortenUrlResponse> IncludeShortenedUrl(ShortenUrlRequest request)
    {
        var code = await GenerateShortCode();
        var shortenedUrl = new ShortenedUrl
        {
            OriginalUrl = request.Url,
            Code = code,
            CreatedAt = DateTime.Now
        };

        await urlShortenerRepository.Add(shortenedUrl);
        await unitOfWork.SaveAsync();
        
        return new ShortenUrlResponse(shortenedUrl.Code);
    }
    
    private async Task<string> GenerateShortCode()
    {
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        while (true)
        {
            var chars = new string(Enumerable.Range(0, Length)
                .Select(_ => characters[Random.Shared.Next(characters.Length)])
                .ToArray());

            if (!await urlShortenerRepository.Exists(chars))
                return chars;
        }
    }

    public async Task<GetOriginalUrlResponse> GetOriginalUrl(string code)
    {
        var shortenedUrl = await urlShortenerRepository.Get(code);

        if (shortenedUrl is not null)
        {
            var visitedUrl = new VisitedUrl
            {
                Code = code,
                VisitedAt = DateTime.Now,
                UserAgent = userContext.UserAgent,
                Referer = userContext.Referer,
                IpAddress = userContext.IpAddress,
                ShortenedUrlId = shortenedUrl.Id
            };
            shortenedUrl.VisitedUrls.Add(visitedUrl);
            
            await urlShortenerRepository.Add(shortenedUrl);
            await unitOfWork.SaveAsync();
        }

        return new GetOriginalUrlResponse(shortenedUrl?.OriginalUrl);
    }
}
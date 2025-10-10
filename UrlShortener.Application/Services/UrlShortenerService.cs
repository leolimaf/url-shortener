using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs.Requests;
using UrlShortener.Application.DTOs.Response;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Services;

public class UrlShortenerService(
    IUrlShortenerRepository urlShortenerRepository,
    IUnitOfWork unitOfWork
    ) : IUrlShortenerService
{
    public const int Length = 7;
    
    public async Task<ShortenUrlResponse> IncludeShortenedUrl(ShortenUrlRequest request, string domain)
    {
        var code = await GenerateShortCode();
        var shortenedUrl = new ShortenedUrl
        {
            Id = Guid.NewGuid(),
            LongUrl = request.Url,
            ShortUrl = $"{domain}/{code}",
            Code = code,
            CreatedAtUtc = DateTime.UtcNow
        };

        await urlShortenerRepository.Add(shortenedUrl);
        await unitOfWork.SaveAsync();
        
        return new ShortenUrlResponse(shortenedUrl.ShortUrl);
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

    public async Task<GetLongUrlResponse> GetLongUrlFromCode(string code)
    {
        var shortenedUrl = await urlShortenerRepository.Get(code);

        return new GetLongUrlResponse(shortenedUrl?.LongUrl);
    }
}
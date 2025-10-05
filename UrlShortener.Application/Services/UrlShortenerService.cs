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
    public const int NumberOfCharactersInShortLink = 7;
    private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    private readonly Random _random = new();
    
    public async Task<string> GenerateUniqueCode()
    {
        while (true)
        {
            var codeChars = new char[NumberOfCharactersInShortLink];
        
            for (var i = 0; i < NumberOfCharactersInShortLink; i++)
                codeChars[i] = Characters[_random.Next(Characters.Length - 1)];
        
            var code = new string(codeChars);

            if (!await urlShortenerRepository.Exists(code))
                return code;
        }
    }
    
    public async Task<ShortenUrlResponse> IncludeShortenedUrl(ShortenUrlRequest request, string domain, string code)
    {
        var shortenedUrl = new ShortenedUrl
        {
            Id = Guid.NewGuid(),
            LongUrl = request.Url,
            ShortUrl = $"{domain}/api/{code}",
            Code = code,
            CreatedAtUtc = DateTime.UtcNow
        };

        await urlShortenerRepository.Add(shortenedUrl);
        await unitOfWork.SaveAsync();
        
        return new ShortenUrlResponse(shortenedUrl.ShortUrl);
    }

    public async Task<GetLongUrlResponse> GetLongUrlFromCode(string code)
    {
        var shortenedUrl = await urlShortenerRepository.Get(code);

        return new GetLongUrlResponse(shortenedUrl?.LongUrl);
    }
}
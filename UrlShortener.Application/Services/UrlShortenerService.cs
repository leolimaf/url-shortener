using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Services;

public class UrlShortenerService(IUrlShortenerRepository urlShortenerRepository) : IUrlShortenerService
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
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs.Auth.Requests;
using UrlShortener.Application.DTOs.Auth.Responses;
using UrlShortener.Application.Settings;
using UrlShortener.Domain.Contracts;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Services;

public class AuthService(
    ILogger<UrlShortenerService> logger,
    IOptions<JwtSettings> jwtOptions,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
    ) : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    public async Task<long> RegisterUser(AddUserRequest request)
    {
        var user = new User
        {
            FullName = $"{request.FirstName} {request.LastName}",
            BirthDate = request.BirthDate,
            Phone = string.Concat(request.Phone.Where(char.IsDigit)),
            Email = request.Email,
            IsEmailConfirmed = false
        };
        
        user.PasswordHash = new PasswordHasher<User>()
            .HashPassword(user, request.Password);
        
        await userRepository.AddUser(user);
        await unitOfWork.SaveAsync();

        return user.Id;
    }

    public async Task<GetUserResponse?> GetUserById(long id)
    {
        var user = await userRepository.GetUserById(id);

        return user is not null 
            ? new GetUserResponse(user.Id, user.FullName, user.BirthDate, user.Phone, user.Email, user.IsEmailConfirmed) 
            : null;
    }

    public async Task<GenarateTokensResponse?> GenarateTokens(GenarateTokensRequest request)
    {
        var user = await userRepository.GetUserByEmail(request.Email);
        
        if (user is null)
            return null;
        
        var passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            return null;
        
        var tokens = GenerateTokens(user);
        
        return new GenarateTokensResponse(tokens.accessToken, tokens.refreshToken);
    }

    public Task<RefreshTokensResponse?> RefreshTokens(RefreshTokensRequest request)
    {
        throw new NotImplementedException();
    }
    
    private (string accessToken, string refreshToken) GenerateTokens(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email)
        };
        
        var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        
        return (tokenHandler.WriteToken(securityToken), Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)));
    }
}
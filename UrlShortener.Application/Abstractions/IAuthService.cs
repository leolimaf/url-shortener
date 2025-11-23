using UrlShortener.Application.DTOs.Auth.Requests;
using UrlShortener.Application.DTOs.Auth.Responses;

namespace UrlShortener.Application.Abstractions;

public interface IAuthService
{
    public Task<long> RegisterUser(AddUserRequest request);
    public Task<GetUserResponse?> GetUserById(long id);
    public Task<GenarateTokensResponse?> GenarateTokens(GenarateTokensRequest request);
    public Task<RefreshTokensResponse?> RefreshTokens(RefreshTokensRequest request);
}
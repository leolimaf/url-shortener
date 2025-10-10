using UrlShortener.Application.Abstractions;

namespace UrlShortener.API.Contexts;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public string? UserAgent => httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();

    public string? Referer => httpContextAccessor.HttpContext?.Request.Headers.Referer.ToString();

    public string? IpAddress => httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
}
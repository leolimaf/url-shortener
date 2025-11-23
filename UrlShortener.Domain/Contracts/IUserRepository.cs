using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Contracts;

public interface IUserRepository
{
    public Task AddUser(User user);
    public Task<User?> GetUserById(long id);
    public Task<User?> GetUserByEmail(string email);
    public Task<User?> GetUserByRefreshTokenHash(string refreshTokenHash);
}
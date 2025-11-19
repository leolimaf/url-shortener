using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Contracts;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task AddUser(User user)
    {
        await context.Users.AddAsync(user);
    }

    public async Task<User?> GetUserById(long id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByRefreshToken(string refreshToken)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.RefreshTokenHash == refreshToken);
    }
}
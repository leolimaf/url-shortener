namespace UrlShortener.Domain.Contracts;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveAsync();
}
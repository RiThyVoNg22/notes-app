using NotesApi.Models;

namespace NotesApi.Services;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User> CreateAsync(string email, string passwordHash, CancellationToken ct = default);
}

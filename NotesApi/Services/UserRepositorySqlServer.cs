using Dapper;
using Microsoft.Data.SqlClient;
using NotesApi.Models;

namespace NotesApi.Services;

public class UserRepositorySqlServer : IUserRepository
{
    private readonly string _connectionString;

    public UserRepositorySqlServer(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT Id, Email, PasswordHash, CreatedAt
            FROM dbo.Users
            WHERE Email = @Email";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<User>(new CommandDefinition(sql, new { Email = email }, cancellationToken: ct));
    }

    public async Task<User> CreateAsync(string email, string passwordHash, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO dbo.Users (Email, PasswordHash, CreatedAt)
            OUTPUT INSERTED.Id, INSERTED.Email, INSERTED.PasswordHash, INSERTED.CreatedAt
            VALUES (@Email, @PasswordHash, GETUTCDATE())";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return await conn.QuerySingleAsync<User>(new CommandDefinition(sql, new { Email = email, PasswordHash = passwordHash }, cancellationToken: ct));
    }
}

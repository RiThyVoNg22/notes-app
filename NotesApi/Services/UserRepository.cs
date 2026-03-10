using Dapper;
using Npgsql;
using NotesApi.Models;

namespace NotesApi.Services;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        const string sql = """
            SELECT "Id", "Email", "PasswordHash", "CreatedAt"
            FROM "Users"
            WHERE "Email" = @Email
            """;
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<User>(new CommandDefinition(sql, new { Email = email }, cancellationToken: ct));
    }

    public async Task<User> CreateAsync(string email, string passwordHash, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO "Users" ("Email", "PasswordHash", "CreatedAt")
            VALUES (@Email, @PasswordHash, NOW())
            RETURNING "Id", "Email", "PasswordHash", "CreatedAt"
            """;
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return await conn.QuerySingleAsync<User>(new CommandDefinition(sql, new { Email = email, PasswordHash = passwordHash }, cancellationToken: ct));
    }
}

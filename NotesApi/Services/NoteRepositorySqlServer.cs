using Dapper;
using Microsoft.Data.SqlClient;
using NotesApi.Models;
using NotesApi.Models.DTOs;

namespace NotesApi.Services;

public class NoteRepositorySqlServer : INoteRepository
{
    private readonly string _connectionString;

    public NoteRepositorySqlServer(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IEnumerable<Note>> GetAllAsync(int? userId, string? search, string? sortBy, bool sortDesc, CancellationToken ct = default)
    {
        var orderColumn = SortColumn(sortBy);
        var orderDir = sortDesc ? "DESC" : "ASC";
        var searchPattern = string.IsNullOrWhiteSpace(search) ? null : $"%{search}%";
        var sql = $@"
            SELECT Id, UserId, Title, Content, CreatedAt, UpdatedAt
            FROM dbo.Notes
            WHERE (@UserId IS NULL OR UserId = @UserId)
              AND (@SearchPattern IS NULL OR Title LIKE @SearchPattern OR Content LIKE @SearchPattern)
            ORDER BY {orderColumn} {orderDir}";

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        var notes = await conn.QueryAsync<Note>(new CommandDefinition(sql, new { UserId = userId, SearchPattern = searchPattern }, cancellationToken: ct));
        return notes;
    }

    public async Task<Note?> GetByIdAsync(int id, int? userId, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT Id, UserId, Title, Content, CreatedAt, UpdatedAt
            FROM dbo.Notes
            WHERE Id = @Id AND (@UserId IS NULL OR UserId = @UserId)";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<Note>(new CommandDefinition(sql, new { Id = id, UserId = userId }, cancellationToken: ct));
    }

    public async Task<Note> CreateAsync(CreateNoteRequest request, int? userId, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO dbo.Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
            VALUES (@UserId, @Title, @Content, GETUTCDATE(), GETUTCDATE());
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        var id = await conn.ExecuteScalarAsync<int>(new CommandDefinition(sql, new
        {
            UserId = userId,
            request.Title,
            Content = request.Content ?? (string?)null
        }, cancellationToken: ct));
        return (await GetByIdAsync(id, userId, ct))!;
    }

    public async Task<Note?> UpdateAsync(int id, UpdateNoteRequest request, int? userId, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE dbo.Notes
            SET Title = @Title, Content = @Content, UpdatedAt = GETUTCDATE()
            WHERE Id = @Id AND (@UserId IS NULL OR UserId = @UserId)";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new { Id = id, UserId = userId, request.Title, Content = request.Content ?? (string?)null }, cancellationToken: ct));
        if (rows == 0) return null;
        return await GetByIdAsync(id, userId, ct);
    }

    public async Task<bool> DeleteAsync(int id, int? userId, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM dbo.Notes WHERE Id = @Id AND (@UserId IS NULL OR UserId = @UserId)";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new { Id = id, UserId = userId }, cancellationToken: ct));
        return rows > 0;
    }

    private static string SortColumn(string? sortBy)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "title" => "Title",
            "updatedat" => "UpdatedAt",
            _ => "CreatedAt"
        };
    }
}

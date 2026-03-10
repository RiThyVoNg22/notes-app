using NotesApi.Models;
using NotesApi.Models.DTOs;

namespace NotesApi.Services;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllAsync(int? userId, string? search, string? sortBy, bool sortDesc, CancellationToken ct = default);
    Task<Note?> GetByIdAsync(int id, int? userId, CancellationToken ct = default);
    Task<Note> CreateAsync(CreateNoteRequest request, int? userId, CancellationToken ct = default);
    Task<Note?> UpdateAsync(int id, UpdateNoteRequest request, int? userId, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, int? userId, CancellationToken ct = default);
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApi.Models.DTOs;
using NotesApi.Services;

namespace NotesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // Users can only access their own notes; userId from JWT enforces this
public class NotesController : ControllerBase
{
    private readonly INoteRepository _repo;

    public NotesController(INoteRepository repo)
    {
        _repo = repo;
    }

    /// <summary>Current user id from JWT. Ensures users only read/update/delete their own notes.</summary>
    private int? GetCurrentUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return id != null && int.TryParse(id, out var uid) ? uid : null;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteResponse>>> GetNotes(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDesc = false,
        CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        var notes = await _repo.GetAllAsync(userId, search, sortBy, sortDesc, ct);
        var list = notes.Select(n => Map(n)).ToList();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NoteResponse>> GetNote(int id, CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        var note = await _repo.GetByIdAsync(id, userId, ct);
        if (note == null) return NotFound();
        return Ok(Map(note));
    }

    [HttpPost]
    public async Task<ActionResult<NoteResponse>> CreateNote([FromBody] CreateNoteRequest request, CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();
        var note = await _repo.CreateAsync(request, userId, ct);
        return CreatedAtAction(nameof(GetNote), new { id = note.Id }, Map(note));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<NoteResponse>> UpdateNote(int id, [FromBody] UpdateNoteRequest request, CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        var note = await _repo.UpdateAsync(id, request, userId, ct);
        if (note == null) return NotFound();
        return Ok(Map(note));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteNote(int id, CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        var deleted = await _repo.DeleteAsync(id, userId, ct);
        if (!deleted) return NotFound();
        return NoContent();
    }

    private static NoteResponse Map(Models.Note n) => new()
    {
        Id = n.Id,
        Title = n.Title,
        Content = n.Content,
        CreatedAt = n.CreatedAt,
        UpdatedAt = n.UpdatedAt
    };
}

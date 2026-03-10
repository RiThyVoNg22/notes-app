using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models.DTOs;

public class CreateNoteRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(10_000)]
    public string? Content { get; set; }
}

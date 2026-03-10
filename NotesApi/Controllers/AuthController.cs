using Microsoft.AspNetCore.Mvc;
using NotesApi.Models.DTOs;
using NotesApi.Services;

namespace NotesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly JwtService _jwt;

    public AuthController(IUserRepository userRepo, JwtService jwt)
    {
        _userRepo = userRepo;
        _jwt = jwt;
    }

    /// <summary>Register a new user. Then use Login to get a JWT.</summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct = default)
    {
        var existing = await _userRepo.GetByEmailAsync(request.Email, ct);
        if (existing != null)
            return BadRequest(new { message = "Email already registered." });

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = await _userRepo.CreateAsync(request.Email, hash, ct);
        var token = _jwt.GenerateToken(user.Id, user.Email);
        return Ok(new AuthResponse { Token = token, UserId = user.Id, Email = user.Email });
    }

    /// <summary>Login: returns JWT. Use Authorization: Bearer &lt;token&gt; on Notes API.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email, ct);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        var token = _jwt.GenerateToken(user.Id, user.Email);
        return Ok(new AuthResponse { Token = token, UserId = user.Id, Email = user.Email });
    }
}

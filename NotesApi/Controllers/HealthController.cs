using Microsoft.AspNetCore.Mvc;

namespace NotesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok", api = "NotesApi" });
}

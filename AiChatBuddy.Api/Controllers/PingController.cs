namespace AiChatBuddy.Api.Controllers;

/// <summary>
/// Prosty endpoint diagnostyczny do sprawdzenia, czy API odpowiada
/// (niezależny od health checków Aspire).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }
}
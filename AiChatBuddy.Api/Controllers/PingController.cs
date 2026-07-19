using Microsoft.AspNetCore.Mvc;

namespace AiChatBuddy.Api.Controllers;

[ApiController]
[Route("")]
public class PingController : ControllerBase
{
    [HttpGet("api/ping")]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }
}
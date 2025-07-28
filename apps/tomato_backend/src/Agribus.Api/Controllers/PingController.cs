using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

[ApiController]
public class PingController : ControllerBase
{
    private readonly ILogger<PingController> _logger;

    public PingController(ILogger<PingController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Endpoints.Ping.Index)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        _logger.LogInformation("Ping received");
        return Ok("pong");
    }

    [HttpGet(Endpoints.Ping.Private)]
    public IActionResult PrivatePing()
    {
        var userId = HttpContext.Items["UserId"]?.ToString();
        _logger.LogInformation("Private Ping received from {UserId}", userId);
        return Ok($"Private Ping received from {userId}");
    }
}

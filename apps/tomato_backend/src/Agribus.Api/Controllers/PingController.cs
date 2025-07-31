using Microsoft.AspNetCore.Authorization;
using Agribus.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

[ApiController]
public class PingController(ILogger<PingController> logger) : ControllerBase
{
    [HttpGet(Endpoints.Ping.Index)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        logger.LogInformation("Ping received");
        return Ok("pong");
    }

    [HttpGet(Endpoints.Ping.Private)]
    public IActionResult PrivatePing()
    {
        var userId = HttpContext.Items["UserId"]!.ToString();
        logger.LogInformation("Private Ping received from {UserId}", userId);
        return Ok($"Private Ping received from {userId}");
    }
}

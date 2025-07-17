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
}

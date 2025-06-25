using Agribus.Core.Ports.Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly ILogger<SensorsController> _logger;

    public SensorsController(ILogger<SensorsController> logger)
    {
        _logger = logger;
        // TODO: add influxdb infra
    }

    [HttpPost("/data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> PostSensorData([FromBody] RawSensorPayload payload)
    {
        // TODO: parse
        // TODO: push in influxdb
        _logger.LogInformation("Received RawSensorPayload", payload);
        return Task.FromResult<IActionResult>(Ok(payload));
    }
}

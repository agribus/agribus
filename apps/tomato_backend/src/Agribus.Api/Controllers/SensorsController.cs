using Agribus.Core.Ports.Api.DTOs;
using Agribus.Core.Ports.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly ILogger<SensorsController> _logger;
    private readonly IParseSensorData _parseSensorData;

    public SensorsController(ILogger<SensorsController> logger, IParseSensorData parseSensorData)
    {
        _logger = logger;
        _parseSensorData = parseSensorData;
        // TODO: add influxdb infra
    }

    [HttpPost("/data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostSensorData(
        [FromBody] RawSensorPayload payload,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation("Received RawSensorPayload: {@Payload}", payload);
        await _parseSensorData.FromRawJson(payload, cancellationToken);
        // TODO: push in influxdb
        return Created();
    }
}

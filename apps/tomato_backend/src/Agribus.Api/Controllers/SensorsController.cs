using Agribus.Application;
using Agribus.Core.Ports.Api.DTOs;
using Agribus.Core.Ports.Api.ParseSensorData;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly ILogger<SensorsController> _logger;
    private readonly SensorDataProcessor _dataProcessor;

    public SensorsController(ILogger<SensorsController> logger, SensorDataProcessor dataProcessor)
    {
        _logger = logger;
        _dataProcessor = dataProcessor;
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
        await _dataProcessor.ProcessAsync(payload, cancellationToken);
        return Created();
    }
}

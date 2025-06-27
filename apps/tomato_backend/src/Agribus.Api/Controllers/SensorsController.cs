using Agribus.Application;
using Agribus.Core.Ports.Api.DTOs;
using Agribus.Core.Ports.Api.ParseSensorData;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

[ApiController]
public class SensorsController : ControllerBase
{
    private readonly ILogger<SensorsController> _logger;
    private readonly SensorDataProcessor _dataProcessor;

    public SensorsController(ILogger<SensorsController> logger, SensorDataProcessor dataProcessor)
    {
        _logger = logger;
        _dataProcessor = dataProcessor;
    }

    [HttpPost(Endpoints.Sensors.PushSensorData)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PushSensorData(
        [FromBody] RawSensorPayload payload,
        CancellationToken cancellationToken
    )
    {
        await _dataProcessor.ProcessAsync(payload, cancellationToken);
        return Created();
    }
}

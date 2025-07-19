using Agribus.Api.Extensions;
using Agribus.Application;
using Agribus.Core.Ports.Api.ParseSensorData.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

[ApiController]
public class SensorsController(ILogger<SensorsController> logger, SensorDataProcessor dataProcessor)
    : ControllerBase
{
    private readonly ILogger<SensorsController> _logger = logger;

    [HttpPost(Endpoints.Sensors.PushSensorData)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PushSensorData(
        [FromBody] RawSensorPayload payload,
        CancellationToken cancellationToken
    )
    {
        await dataProcessor.ProcessAsync(payload, cancellationToken);
        return Created();
    }
}

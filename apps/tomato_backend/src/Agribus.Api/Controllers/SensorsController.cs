using Agribus.Api.Extensions;
using Agribus.Application.SensorUsecases;
using Agribus.Core.Ports.Api.ParseSensorData.DTOs;
using Agribus.Core.Ports.Api.SensorUsecases;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

[ApiController]
public class SensorsController(
    ILogger<SensorsController> logger,
    SensorDataProcessor dataProcessor,
    IUpdateSensorUsecase updateSensorUsecase
) : ControllerBase
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

    [HttpPut(Endpoints.Sensors.UpdateSensor)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditSensor(
        [FromRoute] Guid id,
        [FromBody] UpdateSensorDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var fakeUserId = Guid.NewGuid();
        var updated = await updateSensorUsecase.Handle(id, fakeUserId, dto, cancellationToken);

        return updated != null ? NoContent() : NotFound();
    }
}

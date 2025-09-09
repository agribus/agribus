using Agribus.Api.Extensions;
using Agribus.Application.SensorUsecases;
using Agribus.Core.Ports.Api.ParseSensorData.DTOs;
using Agribus.Core.Ports.Api.SensorUsecases;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;
using Agribus.Core.Ports.Spi.AuthContext;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

[ApiController]
public class SensorsController(
    ILogger<SensorsController> logger,
    SensorDataProcessor dataProcessor,
    IUpdateSensorUsecase updateSensorUsecase,
    IDeleteSensorUsecase deleteSensorUsecase,
    IAuthService authService
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
        [FromBody] UpdateSensorDto? dto,
        CancellationToken cancellationToken = default
    )
    {
        if (dto is null)
            return BadRequest();

        var userId = authService.GetCurrentUserId();
        var updated = await updateSensorUsecase.Handle(id, userId, dto, cancellationToken);

        return updated != null ? NoContent() : NotFound();
    }

    [HttpDelete(Endpoints.Sensors.DeleteSensor)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSensor(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var userId = authService.GetCurrentUserId();
        var deleted = await deleteSensorUsecase.Handle(id, userId, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}

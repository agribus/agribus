using Agribus.Api.Extensions;
using Agribus.Application.GreenhouseUsecases;
using Agribus.Application.SensorUsecases;
using Agribus.Core.Ports.Api.AlertUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Spi.AuthContext;
using Agribus.Core.Ports.Spi.OpenMeteoContext;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

public class GreenhousesController(
    ILogger<GreenhousesController> logger,
    ICreateGreenhouseUsecase createGreenhouseUsecase,
    IDeleteGreenhouseUsecase deleteGreenhouseUsecase,
    IUpdateGreenhouseUsecase updateGreenhouseUsecase,
    IGetUserGreenhousesUsecase getUserGreenhousesUsecase,
    IGetGreenhouseByIdUsecase getGreenhouseByIdUsecase,
    SensorDataProcessor dataProcessor,
    IForecastService forecastService,
    IAuthService authService,
    IGetAlertsByGreenhouseUsecase getAlertsByGreenhouseUsecase
) : ControllerBase
{
    [HttpGet(Endpoints.Greenhouses.GetUserGreenhouseById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserGreenhouseById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var userId = authService.GetCurrentUserId();
        var result = await getGreenhouseByIdUsecase.Handle(id, userId, cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet(Endpoints.Greenhouses.GetGreenhouseForecastById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGreenhouseForecastById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var userId = authService.GetCurrentUserId();
        var greenhouse = await getGreenhouseByIdUsecase.Handle(id, userId, cancellationToken);
        if (greenhouse is null)
            return NotFound();

        var forecast = await forecastService.GetForecastAsync(
            greenhouse.Latitude!,
            greenhouse.Longitude!
        );

        return Ok(forecast);
    }

    [HttpGet(Endpoints.Greenhouses.GetGreenhouseMeasurementsById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGreenhouseMeasurementsById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var userId = authService.GetCurrentUserId();
        var greenhouse = await getGreenhouseByIdUsecase.Handle(id, userId, cancellationToken);
        if (greenhouse is null)
            return NotFound();

        var sourceAdresses = greenhouse.Sensors.Select(sensor => sensor.SourceAddress).ToList();
        var measurements = await dataProcessor.GetMeasurementsAsync(
            sourceAdresses,
            cancellationToken
        );
        return Ok(measurements);
    }

    [HttpGet(Endpoints.Greenhouses.GetUserGreenhouses)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserGreenhouses(CancellationToken cancellationToken)
    {
        var userId = authService.GetCurrentUserId();
        var result = await getUserGreenhousesUsecase.Handle(userId, cancellationToken);
        return Ok(result);
    }

    [HttpPost(Endpoints.Greenhouses.CreateGreenhouse)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGreenhouse(
        [FromBody] CreateGreenhouseDto? dto,
        CancellationToken cancellationToken = default
    )
    {
        if (dto is null)
            return BadRequest();

        var userId = authService.GetCurrentUserId();
        var created = await createGreenhouseUsecase.Handle(dto, userId, cancellationToken);
        return Created(Endpoints.Greenhouses.CreateGreenhouse, created);
    }

    [HttpDelete(Endpoints.Greenhouses.DeleteGreenhouse)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGreenhouse(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var userId = authService.GetCurrentUserId();
        var deleted = await deleteGreenhouseUsecase.Handle(id, userId, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }

    [HttpPut(Endpoints.Greenhouses.EditGreenhouse)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditGreenhouse(
        [FromRoute] Guid id,
        [FromBody] UpdateGreenhouseDto? dto,
        CancellationToken cancellationToken = default
    )
    {
        if (dto is null)
            return BadRequest();

        var userId = authService.GetCurrentUserId();
        var updated = await updateGreenhouseUsecase.Handle(id, userId, dto, cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpGet(Endpoints.Greenhouses.GetGreenhouseAlertsById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGreenhouseAlertsById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var userId = authService.GetCurrentUserId();
        var result = await getAlertsByGreenhouseUsecase.Handle(id, userId, cancellationToken);
        return Ok(result);
    }
}

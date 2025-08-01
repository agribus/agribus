using Agribus.Api.Extensions;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Agribus.Core.Ports.Spi.AuthContext;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

public class GreenhousesController(
    ILogger<GreenhousesController> logger,
    ICreateGreenhouseUsecase createGreenhouseUsecase,
    IDeleteGreenhouseUsecase deleteGreenhouseUsecase,
    IUpdateGreenhouseUsecase updateGreenhouseUsecase,
    IAuthService authService
) : ControllerBase
{
    [HttpPost(Endpoints.Greenhouses.CreateGreenhouse)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGreenhouse(
        [FromBody] CreateGreenhouseDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var userId = authService.GetCurrentUserId();

        try
        {
            var created = await createGreenhouseUsecase.Handle(dto, userId, cancellationToken);
            return Created(Endpoints.Greenhouses.CreateGreenhouse, created);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating greenhouse");
            throw;
        }
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
        [FromBody] UpdateGreenhouseDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var userId = authService.GetCurrentUserId();
        var updated = await updateGreenhouseUsecase.Handle(id, userId, dto, cancellationToken);

        return updated != null ? NoContent() : NotFound();
    }
}

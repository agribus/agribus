using Agribus.Api.Extensions;
using Agribus.Core.Ports.Api.GreenhouseUsecases;
using Agribus.Core.Ports.Api.GreenhouseUsecases.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

public class GreenhousesController(
    ILogger<GreenhousesController> logger,
    ICreateGreenhouseUsecase createGreenhouseUsecase,
    IDeleteGreenhouseUsecase deleteGreenhouseUsecase
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
        // TODO: Authorization when Clerk done
        var fakeUserId = Guid.NewGuid();

        try
        {
            var created = await createGreenhouseUsecase.Handle(dto, fakeUserId, cancellationToken);
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
        // TODO: Authorization when Clerk done
        var fakeUserId = Guid.NewGuid();
        var deleted = await deleteGreenhouseUsecase.Handle(id, fakeUserId, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}

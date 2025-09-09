using Agribus.Api.Extensions;
using Agribus.Core.Ports.Api.AlertUsecases;
using Agribus.Core.Ports.Api.AlertUsecases.DTOs;
using Agribus.Core.Ports.Spi.AuthContext;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

public class AlertsController(
    ILogger<AlertsController> logger,
    ICreateAlertUsecase createAlertUsecase,
    IDeleteAlertUsecase deleteAlertUsecase,
    IAuthService authService
) : ControllerBase
{
    [HttpPost(Endpoints.Alerts.CreateAlert)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAlert(
        [FromBody] CreateAlertDto dto,
        CancellationToken cancellationToken
    )
    {
        var userId = authService.GetCurrentUserId();
        var created = await createAlertUsecase.Handle(dto, userId, cancellationToken);
        return Created(Endpoints.Alerts.CreateAlert, created);
    }

    [HttpDelete(Endpoints.Alerts.DeleteAlert)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAlert(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var userId = authService.GetCurrentUserId();
        var deleted = await deleteAlertUsecase.Handle(id, userId, cancellationToken);
        return NoContent();
    }
}

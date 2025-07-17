using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers;

public class GreenhousesController(ILogger<GreenhousesController> logger) : ControllerBase
{
    private readonly ILogger<GreenhousesController> _logger = logger;
}

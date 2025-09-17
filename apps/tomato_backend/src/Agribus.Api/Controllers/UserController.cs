using Agribus.Api.Extensions;
using Agribus.Api.Middlewares;
using Agribus.Core.Domain.AggregatesModels.AuthAggregates;
using Agribus.Core.Ports.Spi.AuthContext;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IAuthService authService, ILogger<UsersController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost(Endpoints.User.Login)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Tentative de connexion pour l'email: {Email}", request.Email);

            var response = await _authService.LoginAsync(request);

            if (response.Success)
            {
                _logger.LogInformation("Connexion réussie pour l'email: {Email}", request.Email);
                return Ok(response);
            }

            _logger.LogWarning("Échec de connexion pour l'email: {Email}", request.Email);
            return BadRequest(response);
        }

        [HttpPost(Endpoints.User.Signup)]
        public async Task<ActionResult<AuthResponse>> Signup([FromBody] SignupRequest request)
        {
            _logger.LogInformation(
                "Tentative de création de compte pour l'email: {Email}",
                request.Email
            );

            var response = await _authService.SignupAsync(request);

            if (response.Success)
            {
                _logger.LogInformation(
                    "Compte créé avec succès pour l'email: {Email}",
                    request.Email
                );
                return CreatedAtAction(nameof(Signup), response);
            }

            _logger.LogWarning("Échec de création de compte pour l'email: {Email}", request.Email);
            return BadRequest(response);
        }

        [HttpGet(Endpoints.User.Me)]
        public async Task<IActionResult> Me()
        {
            var token = _authService.GetToken();
            var isValid = await _authService.ValidateTokenAsync(token);

            return Ok(new { message = isValid });
        }
    }
}

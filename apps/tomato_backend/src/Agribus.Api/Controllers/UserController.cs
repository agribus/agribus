using Agribus.Clerk.Models;
using Agribus.Clerk.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agribus.Api.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IClerkAuthService _clerkAuthService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IClerkAuthService clerkAuthService, ILogger<UsersController> logger)
        {
            _clerkAuthService = clerkAuthService;
            _logger = logger;
        }

        [HttpPost(Endpoints.User.Login)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Tentative de connexion pour l'email: {Email}",
                    request.Email
                );

                var response = await _clerkAuthService.LoginAsync(request);

                if (response.Success)
                {
                    _logger.LogInformation(
                        "Connexion réussie pour l'email: {Email}",
                        request.Email
                    );
                    return Ok(response);
                }

                _logger.LogWarning("Échec de connexion pour l'email: {Email}", request.Email);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erreur lors de la connexion pour l'email: {Email}",
                    request.Email
                );
                return StatusCode(500, AuthResponse.CreateError("Internal server error"));
            }
        }

        [HttpPost(Endpoints.User.Signup)]
        public async Task<ActionResult<AuthResponse>> Signup([FromBody] SignupRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Tentative de création de compte pour l'email: {Email}",
                    request.Email
                );

                var response = await _clerkAuthService.SignupAsync(request);

                if (response.Success)
                {
                    _logger.LogInformation(
                        "Compte créé avec succès pour l'email: {Email}",
                        request.Email
                    );
                    return CreatedAtAction(nameof(Signup), response);
                }

                _logger.LogWarning(
                    "Échec de création de compte pour l'email: {Email}",
                    request.Email
                );
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erreur lors de la création de compte pour l'email: {Email}",
                    request.Email
                );
                return StatusCode(500, AuthResponse.CreateError("Internal server error"));
            }
        }

        [HttpPost(Endpoints.User.Logout)]
        public ActionResult Logout()
        {
            _logger.LogInformation("Demande de déconnexion reçue");

            return Ok(new { message = "Logout successful" });
        }
    }
}

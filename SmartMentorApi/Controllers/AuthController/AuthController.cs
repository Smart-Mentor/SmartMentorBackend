using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartMentor.Abstraction.Dto.Requests.AuthRequests;
using SmartMentor.Abstraction.Dto.Requests.AuthService;
using SmartMentor.Abstraction.Services.AuthenticationService;

namespace SmartMentorApi.Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService,ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] loginRequest request)
        {
            try
            {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);
            var result = await _authService.LoginAsync(request);
            return Ok(result);
            }catch(Exception ex)
            {
                Log.Error("Error during login: {Message}", ex.Message);
                return StatusCode(500, "An error occurred during login.");
            }
     
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", request.Email);
                var result = await _authService.RegisterAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error("Error during registration: {Message}", ex.Message);
                return StatusCode(500, "An error occurred during registration.");
            }

        }
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                _logger.LogInformation("Password change attempt for user ID: {UserId}", request.UserId);
                var result = await _authService.ChangePasswordAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error("Error during password change: {Message}", ex.Message);
                return StatusCode(500, "An error occurred during password change.");
            }
        }
        [HttpGet("me")]
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                _logger.LogInformation("Fetching profile for the authenticated user.");
                var result = await _authService.GetProfileAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error("Error fetching profile: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while fetching the profile.");
            }
        }
    }
}

using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartMentor.Abstraction.Dto.Requests.AuthRequests;
using SmartMentor.Abstraction.Dto.Requests.AuthService;
using SmartMentor.Abstraction.Services.AuthenticationService;
using SmartMentor.Abstraction.Services.EmailSenderService;

namespace SmartMentorApi.Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailVerificationService _emailVerificationService;

        public AuthController(IAuthService authService,
        ILogger<AuthController> logger,
        IEmailVerificationService emailVerificationService
        
        )
        {
            _authService = authService;
            _logger = logger;
           _emailVerificationService = emailVerificationService;
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
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordRequest request)
        {
            try
            {
                // extract user id from the token
                var userId=HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if(userId == null)
                {
                    return Unauthorized("User ID not found in token.");
                }
                _logger.LogInformation("Password change attempt for user ID: {UserId}", userId);
                var result = await _authService.ChangePasswordAsync(request,userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error("Error during password change: {Message}", ex.Message);
                return StatusCode(500, "An error occurred during password change.");
            }
        }
        [HttpGet("me")]
        [Authorize(Roles = "Student,Mentor,Admin")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                _logger.LogInformation("Fetching profile for the authenticated user.");
                var userId=HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;     
                if(userId == null)
                {
                    return Unauthorized("User ID not found in token.");
                }
                var result = await _authService.GetProfileAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error("Error fetching profile: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while fetching the profile.");
            }
        }
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(string code)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in token.");
                }
                _logger.LogInformation("Email verification attempt for user ID: {UserId}", userId);
                var result = await _emailVerificationService.VerifyCodeAsync(Guid.Parse(userId), code);
                if (result)
                {
                    return Ok(new { Message = "Email verified successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "Invalid or expired verification code." });
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error during email verification: {Message}", ex.Message);
                return StatusCode(500, "An error occurred during email verification.");
            
            }
        }
        [HttpPost("resend-verification-code")]
        public async Task<IActionResult> ResendVerificationCode()
        {
            try
            {
                var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in token.");
                }
                _logger.LogInformation("Resending verification code for user ID: {UserId}", userId);
                await _emailVerificationService.SendVerificationCodeAsync(Guid.Parse(userId));
                return Ok(new { Message = "Verification code resent successfully." });
            }
            catch (Exception ex)
            {
                Log.Error("Error during verification code resend: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while resending the verification code.");
            }
        }
    }
}

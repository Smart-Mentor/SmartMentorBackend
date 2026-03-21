using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("VerificationPolicy")]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody]VerifiyEmailRequest request)
        {
            try
            {
                var result = await _emailVerificationService.VerifyCodeAsync(request.VerificationToken, request.Code);
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
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("VerificationPolicy")]
        [HttpPost("resend-verification-code/{verificationToken}")]
        public async Task<IActionResult> ResendVerificationCode([FromRoute]Guid verificationToken)
        {
            try
            {
               var result= await _emailVerificationService.resendVerificationCodeAsync(verificationToken);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                Log.Error("Error during verification code resend: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while resending the verification code.");
            }
        }
    }
}

using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using smartmentor.abstraction.services.Authenticationservice;
using SmartMentor.Abstraction.Dto.Requests.AuthRequests;
using SmartMentor.Abstraction.Dto.Responses.AuthResponse;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Abstraction.Services.AuthenticationService;
using SmartMentor.Persistence.Identity;
namespace smartmentor.Application.Implementations.AuthenticationService
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IJwtTokenService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleAuthService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public GoogleAuthService(
            IJwtTokenService jwtService, 
            UserManager<ApplicationUser> userManager,
            ILogger<GoogleAuthService> logger,
            IConfiguration configuration,
            IUnitOfWork unitOfWork
            )
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<AuthResponse> AuthenticateWithGoogleAsync(GoogleLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
            {
                _logger.LogWarning("Google authentication failed: IdToken is null or empty.");
                return new AuthResponse(IsSuccessful: false, Message: "Invalid Google token.");
            }

            var googleClientId = _configuration["Authentication:Google:ClientId"];
            if (string.IsNullOrWhiteSpace(googleClientId))
            {
                _logger.LogError("Google authentication failed: Google ClientId is not configured.");
                return new AuthResponse(IsSuccessful: false, Message: "Google authentication is not configured.");
            }

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleClientId }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google token validation failed.");
                return new AuthResponse(IsSuccessful: false, Message: "Invalid Google token.");
            }

            if (payload.ExpirationTimeSeconds < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                _logger.LogWarning(
                    "Google authentication failed: IdToken has expired. Expiration time: {ExpirationTime}, Current time: {CurrentTime}",
                    payload.ExpirationTimeSeconds,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                return new AuthResponse(IsSuccessful: false, Message: "Google token has expired.");
            }

            if (string.IsNullOrWhiteSpace(payload.Email) || payload.EmailVerified != true)
            {
                _logger.LogWarning(
                    "Google authentication failed: Email is missing or not verified for Google subject {Subject}.",
                    payload.Subject);

                return new AuthResponse(IsSuccessful: false, Message: "Google account email is not verified.");
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    FirstName = payload.GivenName ?? string.Empty,
                    LastName = payload.FamilyName ?? string.Empty,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailVerifiedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create user for Google authentication: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return new AuthResponse(IsSuccessful: false, Message: "Failed to create user for Google authentication.");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "Student");
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Failed to assign Student role for Google authentication: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    return new AuthResponse(IsSuccessful: false, Message: "Failed to assign default role for Google authentication.");
                }

                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                _logger.LogInformation("User with email {Email} already exists. Proceeding with authentication.", payload.Email);
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;
            var token = await _jwtService.GenerateTokenAsync(user);

            return new AuthResponse(
                IsSuccessful: true,
                Message: "Authentication successful.",
                Token: token,
                User: new UserResponse(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    role,
                    true,
                    "Authentication successful.",
                    user.EmailConfirmed
                )
            );
        }
    }
}

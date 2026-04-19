using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Dto.Requests.AuthRequests;
using SmartMentor.Abstraction.Dto.Requests.AuthResponse;
using SmartMentor.Abstraction.Dto.Requests.AuthService;
using SmartMentor.Abstraction.Dto.Responses.AuthResponse;
using SmartMentor.Abstraction.Dto.Responses.AuthService;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Abstraction.Services.AuthenticationService;
using SmartMentor.Abstraction.Services.EmailSenderService;
using SmartMentor.Domain.Entiies;
using SmartMentor.Persistence.Identity;
namespace SmartMentor.Application.Implementations.AuthenticationService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly RoleManager<ApplicationRole> _roleManager;

        private readonly ILogger<AuthService> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtToken;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordResetService _passwordResetService;

        public AuthService(UserManager<ApplicationUser>userManger,
            RoleManager<ApplicationRole> roleManager,
            ILogger<AuthService>logger,
            SignInManager<ApplicationUser>signInManager,
            IJwtTokenService jwtToken,
            IEmailVerificationService emailVerificationService,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            IPasswordResetService passwordResetService
            )
        {
            _userManger = userManger;
            _roleManager = roleManager;
            _logger = logger;
            _signInManager = signInManager;
            _jwtToken = jwtToken;
            _emailVerificationService = emailVerificationService;
           _unitOfWork = unitOfWork;
           _passwordResetService = passwordResetService;
        }

        public async Task<string> ChangePasswordAsync(ChangePasswordRequest request,string UserId)
        {
            try
            {
                if(string.IsNullOrEmpty(UserId))
                {
                    return await Task.FromResult("User ID is missing");
                }
                var user=await _userManger.FindByIdAsync(UserId);
                if(user == null)
                {
                    return await Task.FromResult("User not found");
                }
              // we need to ckeck if the current password equals the client enterd password
          
                var chcekpassword= await _userManger.ChangePasswordAsync(user,request.CurrentPassword,request.NewPassword);
                if(!chcekpassword.Succeeded)
                {
                    var errors = string.Join(", ", chcekpassword.Errors.Select(e => e.Description));
                    _logger.LogWarning("Password change failed for user: {UserId} with errors: {Errors}", UserId, errors);
                    return await Task.FromResult($"Password change failed: {errors}");
                }
                return await Task.FromResult("Password changed successfully");
            }catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred during password change for user ID: {UserId}", UserId);
                return await Task.FromResult("An error occurred during password change");
            }

        }

        public async Task<ForgetPasswordDto> ForgetPasswordAsync(string email)
        {
            try
            {
                var existingEmail = await _userManger.FindByEmailAsync(email);
                if(existingEmail == null)
                {
                    _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
                     return new ForgetPasswordDto
                    {
                        message = "If an account with that email exists, a password reset code has been sent."
                    };
                }
                var result = await _passwordResetService.SendResetCodeAsync(email);
                if (result.IsSuccess)
                {
                    return new ForgetPasswordDto
                    {
                        message = "Password reset code sent to email successfully",
                        resetToken = result.Value
                    };
                }

                _logger.LogWarning("Failed to send password reset code to email: {Email}", email);
                throw new Exception("Failed to send password reset code. Please try again later.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending password reset code to email: {Email}", email);
                throw new Exception("An error occurred while sending password reset code. Please try again later.");
            }        
        }

        public async Task<MeResponse> GetProfileAsync(string UserId)
        {
            try
            {
            _logger.LogInformation("Fetching profile information for the authenticated user.");
            // Get the authenticated user from the context (this is just a placeholder, actual implementation may vary)

            if (string.IsNullOrEmpty(UserId))
            {
                _logger.LogWarning("User ID not found in claims");
               throw new NullReferenceException("User ID not found in claims");
            }
            // fetch the user from the database
            var user=await _userManger.FindByIdAsync(UserId);
            // validate if the user is null
                if(user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", UserId);
                throw new NullReferenceException("User not found");
                }
                // get the roles of the user
                var roles=await _userManger.GetRolesAsync(user);

                var response=new MeResponse
                {
                    UserId= user.Id,
                    Email= user.Email,
                    UserName= user.UserName,
                    FirstName= user.FirstName,
                    LastName= user.LastName,
                    PhoneNumber= user.PhoneNumber,
                    EmailConfirmed= user.EmailConfirmed,
                    PhoneNumberConfirmed= user.PhoneNumberConfirmed, 
                    Roles= roles.ToList()
                };
        
            return response;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching profile information.");
                throw;
                
            }
        }

        public async Task<AuthResponse> LoginAsync(loginRequest request)
        {
            try
            {
                // i need to chcek if the email is already registerd or not 
                _logger.LogInformation("Login attempt for email: {Email}", request.Email);
                var user = await _userManger.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogDebug("There is no account with that email");
                    return new AuthResponse ( IsSuccessful : false, Message : "Invalid email or password" );
                }
                if(user.EmailConfirmed == false)
                {
                    _logger.LogWarning("Login failed - email not confirmed for user: {UserId}", user.Id);
                    return new AuthResponse ( IsSuccessful : false, Message : "Email not confirmed. Please verify your email before logging in." );
                }

                var res = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
                if (!res.Succeeded)
                {
                    _logger.LogWarning("Login failed - invalid password for user: {UserId}", user.Id);
                    return new AuthResponse ( IsSuccessful :false, Message : "Invalid email or password" );
                }
                var token = await _jwtToken.GenerateTokenAsync(user);
                
                return new AuthResponse ( IsSuccessful : true, 
                Message : "Login successful",
                 Token : token ,
                 User : new UserResponse(
                    UserId : user.Id,
                    FirstName : user.FirstName,
                    LastName  : user.LastName,
                    Email : user.Email,
                    Role : (await _userManger.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty,
                    IsSuccessful : true,
                    Message : "User retrieved successfully"
                 ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for email: {Email}", request.Email);
                return new AuthResponse ( IsSuccessful : false, Message : "An error occurred during login" );
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
             // get the email from the request
            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);
            var user=await _userManger.FindByEmailAsync(request.Email);
            if(user != null)
            {
                return new AuthResponse(IsSuccessful: false, Message: "Email is already registered");
            }
            _logger.LogInformation("Creating a new user account for email: {Email}", request.Email);
            var newUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed=false,
                NormalizedEmail=request.Email,
                PhoneNumber=request.PhoneNumber
            };
            var result = await _userManger.CreateAsync(newUser, request.Password);
            await  _userManger.AddClaimAsync(newUser, new System.Security.Claims.Claim("FullName", request.FirstName + " " + request.LastName));
            if(!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("User creation failed for email: {Email} with errors: {Errors}", request.Email, errors);
                return new AuthResponse(IsSuccessful: false, Message: $"User creation failed: {errors}");
            }
            var roleExists = await _roleManager.RoleExistsAsync(request.Role);
            // i want to assign role to the user Student or Mentor
            if(roleExists)
            {
                _logger.LogInformation("Assigning role '{Role}' to user with email: {Email}", request.Role, request.Email);
                await _userManger.AddToRoleAsync(newUser, request.Role);
            }else
            {
                _logger.LogWarning("Role '{Role}' does not exist. Skipping role assignment for user with email: {Email}", request.Role,  request.Email);
            }
            // create the temp Verfication token and save it to the database 
            var verificationToken = Guid.NewGuid();

            _logger.LogInformation("User created a new account with password.");
              await _emailVerificationService.SendVerificationCodeAsync(verificationToken, newUser.Id);
            return new AuthResponse(
                IsSuccessful: true,
                 Message: "User registered successfully, verification code sent to email",
                  VerificationToken: verificationToken,
                 User: new UserResponse(
                    UserId: newUser.Id,
                    FirstName: newUser.FirstName,
                    LastName:  newUser.LastName,
                    Email: newUser.Email,
                    Role: request.Role,
                    IsSuccessful: true,
                    Message: "User registered successfully, verification code sent to email"
                 ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration for email: {Email}", request.Email);
                return new AuthResponse(IsSuccessful: false, Message: "An error occurred during registration");
            }
           
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Dto.Requests.AuthRequests;
using SmartMentor.Abstraction.Dto.Requests.AuthService;
using SmartMentor.Abstraction.Dto.Responses.AuthResponse;
using SmartMentor.Abstraction.Dto.Responses.AuthService;
using SmartMentor.Abstraction.Services.AuthenticationService;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<ApplicationUser>userManger,
            RoleManager<ApplicationRole> roleManager,
            ILogger<AuthService>logger,
            SignInManager<ApplicationUser>signInManager,
            IJwtTokenService jwtToken,
            IHttpContextAccessor httpContextAccessor
            
            )
        {
            _userManger = userManger;
            _roleManager = roleManager;
            _logger = logger;
            _signInManager = signInManager;
            _jwtToken = jwtToken;
            _httpContextAccessor= httpContextAccessor;
        }

        public async Task<string> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var user=await _userManger.FindByIdAsync(request.UserId.ToString());
                if(user == null)
                {
                    return await Task.FromResult("User not found");
                }
          
                var chcekpassword= await _userManger.ChangePasswordAsync(user,request.CurrentPassword,request.NewPassword);
                if(!chcekpassword.Succeeded)
                {
                    var errors = string.Join(", ", chcekpassword.Errors.Select(e => e.Description));
                    _logger.LogWarning("Password change failed for user: {UserId} with errors: {Errors}", request.UserId, errors);
                    return await Task.FromResult($"Password change failed: {errors}");
                }
                return await Task.FromResult("Password changed successfully");
            }catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred during password change for user ID: {UserId}", request.UserId);
                return await Task.FromResult("An error occurred during password change");
            }
          // we need to ckeck if the current password equals the cient enterd password

        }

        public async Task<MeResponse> GetProfileAsync()
        {
            try
            {
            _logger.LogInformation("Fetching profile information for the authenticated user.");
            // Get the authenticated user from the context (this is just a placeholder, actual implementation may vary)
            var currentuser= _httpContextAccessor.HttpContext.User;
            var userId=currentuser.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
               throw new NullReferenceException("User ID not found in claims");
            }
            // fetch the user from the database
            var user=await _userManger.FindByIdAsync(userId);
            // validate if the user is null
                if(user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", userId);
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
                EmailConfirmed=true,
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

            _logger.LogInformation("User created a new account with password.");
            return new AuthResponse(
                IsSuccessful: true,
                 Message: "User registered successfully",
                 User: new UserResponse(
                    UserId: newUser.Id,
                    FirstName: newUser.FirstName,
                    LastName:  newUser.LastName,
                    Email: newUser.Email,
                    Role: request.Role,
                    IsSuccessful: true,
                    Message: "User registered successfully"
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

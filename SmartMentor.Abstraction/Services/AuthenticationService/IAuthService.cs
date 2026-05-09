using SmartMentor.Abstraction.Dto.Requests.AuthRequests;
using SmartMentor.Abstraction.Dto.Requests.AuthResponse;
using SmartMentor.Abstraction.Dto.Requests.AuthService;
using SmartMentor.Abstraction.Dto.Responses.AuthResponse;

namespace SmartMentor.Abstraction.Services.AuthenticationService
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(loginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<string> ChangePasswordAsync(ChangePasswordRequest request, string UserId);
        Task<MeResponse> GetProfileAsync(string userId);
        Task<ForgetPasswordDto> ForgetPasswordAsync(string email);
    }
}

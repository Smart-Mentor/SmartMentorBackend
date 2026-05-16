using SmartMentor.Abstraction.Dto.Requests.AuthRequests;
using SmartMentor.Abstraction.Dto.Responses.AuthResponse;
namespace smartmentor.abstraction.services.Authenticationservice
{
    public interface IGoogleAuthService
    {
        Task<AuthResponse> AuthenticateWithGoogleAsync(GoogleLoginRequest request);
    }
}
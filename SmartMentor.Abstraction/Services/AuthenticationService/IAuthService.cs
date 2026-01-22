using SmartMentor.Abstraction.Dto.Requests.AuthRequests;
using SmartMentor.Abstraction.Dto.Requests.AuthService;
using SmartMentor.Abstraction.Dto.Responses.AuthResponse;
using SmartMentor.Abstraction.Dto.Responses.AuthService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentor.Abstraction.Services.AuthenticationService
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(loginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<string> ChangePasswordAsync(ChangePasswordRequest request);
        Task<MeResponse> GetResponseAsync();
    }
}

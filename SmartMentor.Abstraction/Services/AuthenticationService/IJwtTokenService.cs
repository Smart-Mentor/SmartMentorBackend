using SmartMentor.Abstraction.Dto;
using SmartMentor.Persistence.Identity;

namespace SmartMentor.Abstraction.Services.AuthenticationService
{
    public interface IJwtTokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    }
}

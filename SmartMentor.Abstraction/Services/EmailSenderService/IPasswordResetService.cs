using FluentResults;
using SmartMentor.Abstraction.Dto.Requests.AuthRequests;

namespace SmartMentor.Abstraction.Services.EmailSenderService
{
    public interface IPasswordResetService
    {
        public Task<Result<string>> SendResetCodeAsync(string email);
        public Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
namespace SmartMentor.Abstraction.Services.EmailSenderService
{
    public interface IEmailVerificationService
    {
        public Task SendVerificationCodeAsync(Guid userId);
        public Task<bool> VerifyCodeAsync(Guid userId, string code);
    }
}
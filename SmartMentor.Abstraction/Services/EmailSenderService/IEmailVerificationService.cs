namespace SmartMentor.Abstraction.Services.EmailSenderService
{
    public interface IEmailVerificationService
    {
        public  Task SendVerificationCodeAsync(Guid verficationtoken, Guid userId);
        public Task<Guid> GetOrCreateActiveVerificationTokenAsync(Guid userId, CancellationToken cancellationToken = default);
        public Task<bool> VerifyCodeAsync(Guid verficationtoken, string code);

        public Task<string> resendVerificationCodeAsync(Guid verficationtoken);

    }
}

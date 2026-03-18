using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Abstraction.Services.EmailSenderService;
using SmartMentor.Domain.Entiies;
using SmartMentor.Persistence.Identity;

namespace SmartMentor.Application.Implementations.AuthenticationService.EmailVerficationService
{
    public class EmailVerficationService : IEmailVerificationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EmailVerficationService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSenderService _emailSenderService;

        public EmailVerficationService(UserManager<ApplicationUser>userManager,
        ILogger<EmailVerficationService> logger
        ,IUnitOfWork unitOfWork,
        IEmailSenderService emailSenderService
        )
        {
            _userManager = userManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _emailSenderService = emailSenderService;
        }
        public async Task SendVerificationCodeAsync(Guid userId)
        {
            // i need to save the code in the database with the user id and expiration date
            // then i need to send the code to the user's email
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null || string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException("User or user email not found while sending verification code.");
            }

            var code = GenerateVerificationCode();
            _logger.LogInformation($"Generated verification code: ***** for userId: {userId}");
            // Save the code to the database
            var emailVerificationCode = new EmailVerificationCodes
            {
                Code = code,
                ExpirationDate = DateTime.UtcNow.AddMinutes(10), // Code expires in 10 minutes
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            await _unitOfWork.Repository<EmailVerificationCodes>().AddAsync(emailVerificationCode);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Saved verification code to database for userId: {userId}");
            await _emailSenderService.SendEmailAsync(
                user.Email,
                "SmartMentor email verification code",
                $"Your verification code is: {code}. It will expire in 10 minutes.");
            _logger.LogInformation("Verification email sent to userId: {UserId}", userId);

        }
        public async Task<bool> VerifyCodeAsync(Guid userId, string code)
        {
            // i need to check if the code is valid and not expired and not used
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)            {
                _logger.LogWarning($"Verification failed: User not found for userId: {userId}");
                return false;
            }
            var verificationCodes = await _unitOfWork.Repository<EmailVerificationCodes>()
                .FindAsync(ev => ev.UserId == userId && ev.Code == code && !ev.IsUsed);
            var verificationCode = verificationCodes.FirstOrDefault();

            if (verificationCode == null)
            {
                _logger.LogWarning($"Verification failed: No matching code found for userId: {userId}");
                return false;
            }
            // Check if the code is expired
            if (verificationCode.ExpirationDate < DateTime.UtcNow)
            {
                _logger.LogWarning($"Verification failed: Code expired for userId: {userId}");
                return false;
            }

            verificationCode.IsUsed = true;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            _unitOfWork.Repository<EmailVerificationCodes>().Update(verificationCode);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Verification succeeded for userId: {UserId}", userId);

            return true;

        }
        private string GenerateVerificationCode()
        {
            // Generate a random 6-digit code
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

    }
}
using FluentResults;
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

        public async Task<string> resendVerificationCodeAsync(Guid verficationtoken)
        {
            var record = await _unitOfWork.Repository<EmailVerificationCodes>()
            .FindAsync(ev => ev.VerficationToken == verficationtoken);
            var entity = record.FirstOrDefault();

            var user = await _userManager.FindByIdAsync(entity.UserId.ToString());

            if(user.EmailConfirmed)
            {
                _logger.LogInformation($"User with userId: {user.Id} already verified their email, no need to resend code.");
                return "Email already verified, no need to resend code.";
            }

            if (entity == null)
            {
                _logger.LogWarning($"Resend verification code failed: No matching record found for verficationtoken: {verficationtoken}");
                throw new Exception("Invalid verification token.");
            }

            entity.Code = GenerateCode();
            entity.ExpirationDate = DateTime.UtcNow.AddMinutes(10);
            entity.IsUsed = false;
            entity.CreatedAt = DateTime.UtcNow;
            
            _unitOfWork.Repository<EmailVerificationCodes>().Update(entity);
            await _unitOfWork.SaveChangesAsync();

            if (user != null)
            {
                await _emailSenderService.SendEmailAsync(
                    user.Email,
                    "SmartMentor email verification code",
                    $"Your new verification code is: {entity.Code}. It will expire in 10 minutes.");
                _logger.LogInformation("Verification email resent to userId: {UserId}", user.Id);
                     return "Verification code resent successfully.";
            }else
            {
                _logger.LogWarning($"Resend verification code failed: No user found for userId: {entity.UserId}");
                throw new Exception("User not found.");
            }

        }

    
        public async Task SendVerificationCodeAsync(Guid verficationtoken,Guid userId)
        {
           var code = GenerateCode();
            var user = await _userManager.FindByIdAsync(userId.ToString());
             if(user == null)
            {
                _logger.LogWarning($"Send verification code failed: No user found for userId: {userId}");
                throw new Exception("User not found.");
            }
             if(user.EmailConfirmed)
            {
                _logger.LogInformation($"User with userId: {user.Id} already verified their email, no need to send code.");
                throw new Exception("Email already verified, no need to send code.");
            }
            // 1. Map everything to the entity at once
            var verificationRecord = new EmailVerificationCodes
            {
                VerficationToken = verficationtoken,
                UserId = userId, // Ensure this property exists in your entity!
                Code = code,
                ExpirationDate = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            // 2. Save once
            await _unitOfWork.Repository<EmailVerificationCodes>().AddAsync(verificationRecord);
            await _unitOfWork.SaveChangesAsync();

            // 3. Send the email
            _logger.LogInformation("Saved verification code for userId: {UserId}", user.Id);
            
            await _emailSenderService.SendEmailAsync(
                user.Email,
                "SmartMentor Email Verification Code",
                $"Your verification code is: {code}. It will expire in 10 minutes.");

            _logger.LogInformation("Verification email sent to: {Email}", user.Email);

        }
        public async Task<bool> VerifyCodeAsync(Guid verficationtoken, string code)
        {
            // i need to check if the code is valid and not expired and not used
            var emailVerificationCodes = await _unitOfWork.Repository<EmailVerificationCodes>()
                .FindAsync(ev => ev.VerficationToken == verficationtoken && ev.Code == code && !ev.IsUsed);
            var emailVerificationCode = emailVerificationCodes.FirstOrDefault();
            if (emailVerificationCode == null)
            {
                _logger.LogWarning($"Verification failed: No matching code found for verficationtoken: {verficationtoken}");
                return false;
            }
            if(emailVerificationCode.IsUsed)
            {
                _logger.LogWarning($"Verification failed: Code already used for verficationtoken: {verficationtoken}");
                return false;
            }
            // Check if the code is expired
            if (emailVerificationCode.ExpirationDate < DateTime.UtcNow)
            {
                _logger.LogWarning($"Verification failed: Code expired for verficationtoken: {verficationtoken}");
                return false;
            }

            emailVerificationCode.IsUsed = true;
            // Note: You'll need to fetch the user object to update their email confirmation status
            var user = await _userManager.FindByIdAsync(emailVerificationCode.UserId.ToString());
            if (user != null)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }
            _unitOfWork.Repository<EmailVerificationCodes>().Update(emailVerificationCode);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Verification succeeded for userId: {UserId}",user.Id);

            return true;

        }
        public static string GenerateCode()
        {
            // Generate a random 6-digit code
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }


       
    }
}
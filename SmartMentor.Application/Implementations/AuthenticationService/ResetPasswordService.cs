using FluentResults;
using SmartMentor.Abstraction.Dto.Requests.AuthRequests;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Abstraction.Services.EmailSenderService;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using SmartMentor.Persistence.Identity;
using SmartMentor.Domain.Entiies;
namespace SmartMentor.Application.Implementations.AuthenticationService
{
    public class ResetPasswordService : IPasswordResetService
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly ILogger<ResetPasswordService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordService(IEmailSenderService emailSenderService,
        ILogger<ResetPasswordService> logger,
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork
        )
        {
            _emailSenderService = emailSenderService;
            _logger = logger;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                if (request.NewPassword != request.ConfirmPassword)
                {
                    return Result.Fail("New password and confirmation password do not match.");
                }

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning($"Password reset failed: No user found for email: {request.Email}");
                    return Result.Fail("Invalid reset request.");
                }

                var codeRecords = await _unitOfWork.Repository<PasswordResetCodes>().FindAsync(
                    x => x.UserId == user.Id
                    && x.ResetToken == request.ResetToken
                    && x.Code == request.Code
                    && !x.IsUsed);

                var codeRecord = codeRecords
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefault();

                if (codeRecord == null)
                {
                    _logger.LogWarning("Password reset failed: invalid token/code for email: {Email}", request.Email);
                    return Result.Fail("Invalid reset request.");
                }

                if (codeRecord.Expiration < DateTime.UtcNow)
                {
                    _logger.LogWarning("Password reset failed: expired token/code for email: {Email}", request.Email);
                    return Result.Fail("Reset code expired.");
                }

                var identityToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var identityResult = await _userManager.ResetPasswordAsync(user, identityToken, request.NewPassword);
                if (!identityResult.Succeeded)
                {
                    var errors = string.Join(", ", identityResult.Errors.Select(x => x.Description));
                    _logger.LogWarning("Password reset failed for email: {Email}. Errors: {Errors}", request.Email, errors);
                    return Result.Fail(errors);
                }

                codeRecord.IsUsed = true;
                _unitOfWork.Repository<PasswordResetCodes>().Update(codeRecord);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Password reset completed successfully for email: {Email}", request.Email);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset password for email: {Email}", request.Email);
                return Result.Fail("An error occurred while resetting password.");
            }
        }

        public async Task<Result<string>> SendResetCodeAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Forgot password requested for non-existing email: {Email}", email);
                    return Result.Fail("Invalid reset request.");
                }

                var code = GenerateCode();
                var resetToken = Guid.NewGuid();
                var passwordResetRecord = new PasswordResetCodes
                {
                    UserId = user.Id,
                    Code = code,
                    ResetToken = resetToken,
                    CreatedAt = DateTime.UtcNow,
                    Expiration = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false
                };

                await _unitOfWork.Repository<PasswordResetCodes>().AddAsync(passwordResetRecord);
                await _unitOfWork.SaveChangesAsync();

                await _emailSenderService.SendEmailAsync(
                    email,
                    "SmartMentor Password Reset Code",
                    $"Your password reset code is: {code}. It will expire in 10 minutes.");

                _logger.LogInformation("Password reset code sent successfully to email: {Email}", email);
                return Result.Ok(resetToken.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset code to email: {Email}", email);
                return Result.Fail("Failed to send password reset code. Please try again later.");
            }
        }

        private static string GenerateCode()
        {
            return Random.Shared.Next(100000, 999999).ToString();
        }
    }

}
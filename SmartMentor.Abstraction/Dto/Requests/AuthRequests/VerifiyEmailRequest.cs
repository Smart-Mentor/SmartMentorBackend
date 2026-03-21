using System.ComponentModel.DataAnnotations;

namespace SmartMentor.Abstraction.Dto.Requests.AuthRequests
{
    public record VerifiyEmailRequest(
          [Required(ErrorMessage = "Verification token is required")]              
        Guid VerificationToken,
        [Required(ErrorMessage = "Verification code is required")]
        string Code
    );
}
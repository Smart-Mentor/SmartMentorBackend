using System.ComponentModel.DataAnnotations;

namespace SmartMentor.Abstraction.Dto.Requests.AuthRequests
{
    public record ForgetPasswordRequest(
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        string Email
    );
}

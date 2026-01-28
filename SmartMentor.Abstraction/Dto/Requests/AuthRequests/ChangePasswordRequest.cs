using System.ComponentModel.DataAnnotations;

namespace SmartMentor.Abstraction.Dto.Requests.AuthRequests
{
public record ChangePasswordRequest(

    [Required(ErrorMessage = "Current password is required.")]
    string CurrentPassword,
    [Required(ErrorMessage = "New password is required.")]
    string NewPassword

);
}
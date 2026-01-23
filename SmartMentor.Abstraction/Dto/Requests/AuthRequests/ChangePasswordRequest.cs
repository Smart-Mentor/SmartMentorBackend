using System.ComponentModel.DataAnnotations;

namespace SmartMentor.Abstraction.Dto.Requests.AuthRequests
{
public record ChangePasswordRequest(

    [Required(ErrorMessage = "UserId is required.")]
    Guid UserId,
    [Required(ErrorMessage = "Current password is required.")]
    string CurrentPassword,
    [Required(ErrorMessage = "New password is required.")]
    string NewPassword

);
}
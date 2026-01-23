using System.ComponentModel.DataAnnotations;

namespace SmartMentor.Abstraction.Dto.Requests.AuthRequests
{
public record ChangePasswordRequest(
    [Required] Guid UserId,
    [Required] string CurrentPassword,
    [Required] string NewPassword
);
}
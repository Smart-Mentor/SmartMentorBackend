namespace SmartMentor.Abstraction.Dto.Requests.AuthRequests
{
public record ChangePasswordRequest(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
);
}
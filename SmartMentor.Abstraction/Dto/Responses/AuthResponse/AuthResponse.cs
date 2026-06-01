

namespace SmartMentor.Abstraction.Dto.Responses.AuthResponse
{
    public record AuthResponse(
        bool IsSuccessful,
        string? Message = null,
        Guid? VerificationToken = null,
        string? Token = null,
        UserResponse? User = null
    );
}
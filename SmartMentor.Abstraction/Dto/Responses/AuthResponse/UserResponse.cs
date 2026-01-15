
using System.ComponentModel.DataAnnotations;

// SmartMentor.Abstraction/Dto/Responses/AuthResponse/UserResponse.cs
namespace SmartMentor.Abstraction.Dto.Responses.AuthResponse
{
    public record UserResponse(
        Guid UserId,
        string FirstName,
        string LastName,
        [Required(ErrorMessage = "Email is required.")]
        string Email,
        string Role,
        bool IsSuccessful,
        string Message
    );
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// SmartMentor.Abstraction/Dto/Responses/AuthResponse/UserResponse.cs
namespace SmartMentor.Abstraction.Dto.Responses.AuthResponse
{
    public record UserResponse(
        Guid UserId,
        string FirstName,
        string LastName,
        string Email,
        string Role,
        bool IsSuccessful,
        string Message
    );
}

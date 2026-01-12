using SmartMentor.Abstraction.Dto.Responses.AuthResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// SmartMentor.Abstraction/Dto/Responses/AuthResponse/AuthResponse.cs
namespace SmartMentor.Abstraction.Dto.Responses.AuthService
{
    public record AuthResponse(
        bool IsSuccessful,
        string Message,
        string? Token = null,
        UserResponse? User = null
    );
}
using System.ComponentModel.DataAnnotations;
namespace SmartMentor.Abstraction.Dto.Requests.AuthRequests
{
    public class GoogleLoginRequest
    {
        [Required(ErrorMessage = "IdToken is required.")]
        public required string IdToken { get; set; }

    }
}

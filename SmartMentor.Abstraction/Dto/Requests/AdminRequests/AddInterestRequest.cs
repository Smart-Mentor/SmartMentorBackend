using System.ComponentModel.DataAnnotations;

namespace SmartMentor.Abstraction.Dto.Requests.AdminRequests
{
    public class AddInterestRequest
    {
        [Required(ErrorMessage = "Interest name is required.")]
        public string Name { get; set; }
      
    }
}
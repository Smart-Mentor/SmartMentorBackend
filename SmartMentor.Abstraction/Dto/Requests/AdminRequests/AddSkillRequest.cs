using System.ComponentModel.DataAnnotations;

namespace SmartMentor.Abstraction.Dto.Requests.AdminRequests
{
    public class AddSkillRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; }
    }
}
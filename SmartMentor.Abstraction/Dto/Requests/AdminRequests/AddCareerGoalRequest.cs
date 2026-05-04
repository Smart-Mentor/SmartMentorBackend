using System.ComponentModel.DataAnnotations;

namespace SmartMentor.Abstraction.Dto.Requests.AdminRequests
{
public class AddCareerGoalRequest
    {
        [Required(ErrorMessage = "Career Goal Name is required")]
        [StringLength(100, ErrorMessage = "Career Goal Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
    }
}
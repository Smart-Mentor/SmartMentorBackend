using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentor.Abstraction.Dto.Requests.AuthService
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string? PhoneNumber { get; set; }

       [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
    }
}

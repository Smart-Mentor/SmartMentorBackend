using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentor.Abstraction.Dto.Responses.AuthResponse
{
    public class ForgetPasswordDto
    {
        public string resetToken {  get; set; }
        
        public string message {  get; set; }
    }
}

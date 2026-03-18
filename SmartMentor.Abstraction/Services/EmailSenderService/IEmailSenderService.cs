using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentor.Abstraction.Services.EmailSenderService
{
    public interface IEmailSenderService
    {
        Task<string> SendEmailAsync(string to, string subject, string body);

    }
}

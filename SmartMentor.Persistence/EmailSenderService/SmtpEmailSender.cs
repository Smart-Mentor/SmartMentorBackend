using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using SmartMentor.Abstraction.Services.EmailSenderService;

namespace SmartMentor.Application.Implementations.EmailSenderService
{
    public class SmtpEmailSender : IEmailSenderService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
            // Implement your SMTP email sending logic here
            // You can use libraries like System.Net.Mail or third-party libraries like MailKit
            var port = _configuration.GetSection("EmailSettings:Port").Value
                ?? throw new InvalidOperationException("EmailSettings:Port is missing.");
            var smtpServer = _configuration.GetSection("EmailSettings:SmtpServer").Value
                ?? throw new InvalidOperationException("EmailSettings:SmtpServer is missing.");
            var username = _configuration.GetSection("EmailSettings:Username").Value
                ?? throw new InvalidOperationException("EmailSettings:Username is missing.");
            var password = _configuration.GetSection("EmailSettings:Password").Value
                ?? throw new InvalidOperationException("EmailSettings:Password is missing.");
            var fromEmail = _configuration.GetSection("EmailSettings:SenderEmail").Value
                ?? throw new InvalidOperationException("EmailSettings:SenderEmail is missing.");
            var fromName = _configuration.GetSection("EmailSettings:SenderName").Value
                ?? throw new InvalidOperationException("EmailSettings:SenderName is missing.");

            var email= new MimeMessage();
            email.From.Add(new MailboxAddress(fromName, fromEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            if (!int.TryParse(port, out var smtpPort))
            {
                throw new InvalidOperationException("EmailSettings:Port is not a valid number.");
            }
            // connct to the SMTP server and send the email
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(username, password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
            return "Email sent successfully.";
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return $"Failed to send email: {ex.Message}";   
            }
          
        }
    }
}
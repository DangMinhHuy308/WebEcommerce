
using System.Net.Mail;

namespace WebEcommerce.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("testnguyenvana1@gmail.com", "zcqt qfyz sohj fngf")
            };
            return client.SendMailAsync(
                new MailMessage("testnguyenvana1@gmail.com", email, subject, message));
        }
    }
}

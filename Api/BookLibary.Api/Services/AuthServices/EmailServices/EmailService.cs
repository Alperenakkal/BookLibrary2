using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BookLibary.Api.Models;

namespace BookLibary.Api.Services.AuthServices.EmailServices
{
    public class EmailService: IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("booklibraryv@gmail.com", "ufobbuydbkzusvsh"),
                EnableSsl = true,
            };
        }

        public async Task SendEmailAsync(Email Email)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(Email.EmailAddress),
                Subject = "Book Library Destek ",
                Body = $@"
                    Name: {Email.Name}
                    Email: {Email.EmailAddress}
                    Phone: {Email.Phone}
                    Message: {Email.Message}
                ",
                IsBodyHtml = false,
            };
           

            mailMessage.To.Add("booklibraryv@gmail.com"); // Alıcı e-posta adresinizi girin
    
            await _smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendVerifyEmailAsync(Email Email)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(Email.EmailAddress),
                Subject = "Book Library Destek ",
                Body = Email.HtmlContent,
                IsBodyHtml = true,
            };


            mailMessage.To.Add(Email.EmailAddress); // Alıcı e-posta adresinizi girin

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
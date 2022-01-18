using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MovieProMVC.Models.ViewModels;
using MovieProMVC.Services.Interfaces;

namespace MovieProMVC.Services
{
    public class EmailService : IMovieEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendContactEmailAsync(string name, string emailFrom, string subject, string htmlMessage)
        {
            var mail = new MimeMessage();
            mail.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email);
            mail.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));
            mail.To.Add(MailboxAddress.Parse(_mailSettings.Email));
            mail.Subject = subject;

            var builder = new BodyBuilder()
            {
                HtmlBody = $"<b>{name}</b> has sent you an email and can be reached at: <b>{emailFrom}</b><br/><br/>{htmlMessage}"
            };
            mail.Body = builder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.User, _mailSettings.Password);

                await smtp.SendAsync(mail);

                smtp.Disconnect(true);
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mail = new MimeMessage();
            mail.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email);
            mail.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));
            mail.To.Add(MailboxAddress.Parse(email));
            mail.Subject = subject;

            var builder = new BodyBuilder() { HtmlBody = htmlMessage };
            mail.Body = builder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.User, _mailSettings.Password);

                await smtp.SendAsync(mail);

                smtp.Disconnect(true);
            }
        }
    }
}

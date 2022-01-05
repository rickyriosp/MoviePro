using Microsoft.AspNetCore.Identity.UI.Services;

namespace MovieProMVC.Services
{
    public class EmailSenderService : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }
}

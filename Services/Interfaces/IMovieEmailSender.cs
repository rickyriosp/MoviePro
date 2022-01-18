using Microsoft.AspNetCore.Identity.UI.Services;

namespace MovieProMVC.Services.Interfaces
{
    public interface IMovieEmailSender : IEmailSender
    {
        Task SendContactEmailAsync(string name, string emailFrom, string subject, string htmlMessage);
    }
}

namespace MovieProMVC.Models.ViewModels
{
    public class MailSettings
    {
        // Configure and use an SMTP server: from Google for example
        public string User { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
    }
}

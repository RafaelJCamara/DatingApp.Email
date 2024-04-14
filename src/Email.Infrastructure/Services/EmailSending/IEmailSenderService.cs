namespace Email.Infrastructure.Services.EmailSending
{
    public interface IEmailSenderService
    {
        Task<string> SendUserEmailValidationEmail(string userEmail);
    }
}

namespace Email.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> ValidateEmail(string emailValidationCode);
    }
}

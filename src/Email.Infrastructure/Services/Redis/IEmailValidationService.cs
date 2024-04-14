using Email.Domain.Models;

namespace Email.Infrastructure.Services.Redis
{
    public interface IEmailValidationService
    {
        Task AddNewEmailToValidate(string emailValidationCode, string username, string userEmail);
        Task<IEnumerable<UserEmail>> BlockEmails();
    }
}

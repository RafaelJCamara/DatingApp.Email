using Email.Domain.Constants;
using Email.Infrastructure.Settings;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Options;

namespace Email.Infrastructure.Services.EmailSending
{
    public class EmailSenderService : IEmailSenderService
    {
        private IMailjetClient _mailClient;
        private EmailSettings _emailSettings;

        public EmailSenderService(IMailjetClient mailClient, IOptions<EmailSettings> emailSettings)
        {
            _mailClient = mailClient;
            _emailSettings = emailSettings.Value;
        }

        public async Task<string> SendUserEmailValidationEmail(string userEmail)
        {
            string emailValidationCode = Guid.NewGuid().ToString("N");

            var email = new TransactionalEmailBuilder()
                   .WithFrom(new SendContact(_emailSettings.SenderEmail))
                   .WithSubject("DatingApp - Activate your account")
                   .WithHtmlPart($"<h1>Confirm your email</h1><div>We just need to validate your email address to activate your Love Connect account. Simply click the following button: </div> <div><a href=\"https://localhost:4200/email-validation/{emailValidationCode}\"> <button>Activate my account</button></a></div> <div>If the link doesn’t work, copy this URL into your browser:</div><div>https://localhost:4200/email-validation/{emailValidationCode}</div><div><strong>Note:</strong> You only have {EmailConstants.MaxDaysToValidateEmailOnceRegistered} days to validate your account. Otherwise, it will be blocked.</div>")
                   .WithTo(new SendContact(userEmail))
                   .Build();

            await _mailClient.SendTransactionalEmailAsync(email);

            return emailValidationCode;
        }
    }
}

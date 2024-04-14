using DatingApp.Common.Events;
using Email.Infrastructure.Services.EmailSending;
using Email.Infrastructure.Services.Redis;
using MassTransit;

namespace Email.Infrastructure.Events.Handlers
{
    public class UserCreatedEventHandler : IConsumer<UserCreatedEvent>
    {
        private IEmailSenderService _emailService;
        private IEmailValidationService _emailValidationService;

        public UserCreatedEventHandler(IEmailSenderService emailService, IEmailValidationService emailValidationService)
        {
            _emailService = emailService;
            _emailValidationService = emailValidationService;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var emailValidationCode = await _emailService.SendUserEmailValidationEmail(context.Message.Email);
            await _emailValidationService.AddNewEmailToValidate(emailValidationCode, context.Message.Username, context.Message.Email);
        }
    }
}

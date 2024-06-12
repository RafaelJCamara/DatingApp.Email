using System.Diagnostics;
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
            var activitySource = new ActivitySource("DatingApp.Email");
            using (var activity =
                   activitySource.StartActivity("[Consumer] User email not validated in grace period event",
                       ActivityKind.Consumer))
            {
                activity?.SetTag("messaging.system", "rabbitmq");
                activity?.SetTag("messaging.origin_kind", "queue");
                activity?.SetTag("messaging.rabbitmq.queue", "user-created-event");
                
                var emailValidationCode = await _emailService.SendUserEmailValidationEmail(context.Message.Email);
                await _emailValidationService.AddNewEmailToValidate(emailValidationCode, context.Message.Username, context.Message.Email);
            }
        }
    }
}

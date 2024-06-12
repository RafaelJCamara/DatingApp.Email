using System.Diagnostics;
using DatingApp.Common.Events;
using Email.Infrastructure.Services.Redis;
using MassTransit;
using Quartz;

namespace Email.Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class EmailVerificationJob : IJob
    {
        private readonly IEmailValidationService _emailValidationService;
        private readonly IPublishEndpoint _publishEndpoint;

        public EmailVerificationJob(IEmailValidationService emailValidationService, IPublishEndpoint publishEndpoint)
        {
            _emailValidationService = emailValidationService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var blockedEmails = await _emailValidationService.BlockEmails();
            
            var activitySource = new ActivitySource("DatingApp.Email");

            using (var activity =
                   activitySource.StartActivity("[Producer] User email not validated in grace period event",
                       ActivityKind.Producer))
            {
                foreach (var blockedEmail in blockedEmails)
                {
                    activity?.SetTag("messaging.system", "rabbitmq");
                    activity?.SetTag("messaging.destination_kind", "queue");
                    activity?.SetTag("messaging.rabbitmq.queue", "user-email-not-validated-in-grace-period-event");
                    await _publishEndpoint.Publish(new UserEmailNotValidatedInGracePeriodEvent
                    {
                        Username = blockedEmail.Username,
                        Email = blockedEmail.Email
                    });
                }   
            }
        }
    }
}

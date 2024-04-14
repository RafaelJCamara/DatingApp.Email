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

            foreach (var blockedEmail in blockedEmails)
            {
                await _publishEndpoint.Publish(new UserEmailNotValidatedInGracePeriodEvent
                {
                    Username = blockedEmail.Username,
                    Email = blockedEmail.Email
                });
            }
        }
    }
}

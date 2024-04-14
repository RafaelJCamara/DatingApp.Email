using DatingApp.Common.Events;
using DatingApp.Common.Helpers.User;
using Email.Application.Services.Interfaces;
using Email.Domain.Models;
using MassTransit;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Email.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly IDatabase _redis;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUser _currentUser;

        public EmailService(IDatabase redis, IPublishEndpoint publishEndpoint, IUser currentUser)
        {
            _redis = redis;
            _publishEndpoint = publishEndpoint;
            _currentUser = currentUser;
        }

        public async Task<bool> ValidateEmail(string emailValidationCode)
        {
            var redisUserEmail = await _redis.StringGetAsync(emailValidationCode);

            if (redisUserEmail.HasValue)
            {
                var userEmail = JsonConvert.DeserializeObject<UserEmail>(redisUserEmail.ToString());

                if (!userEmail.Username.ToLowerInvariant().Equals(_currentUser.Username.ToLowerInvariant())) return false;

                await _publishEndpoint.Publish(new EmailValidatedEvent
                {
                    Email = userEmail.Email,
                    Username = userEmail.Username
                });

                var existingEmailsToValidate = JsonConvert.DeserializeObject<List<string>>((await _redis.StringGetAsync("emailKeys")).ToString());

                if (existingEmailsToValidate is not null)
                {
                    existingEmailsToValidate.Remove(emailValidationCode);
                    if (existingEmailsToValidate.Count == 0)
                        await _redis.KeyDeleteAsync("emailKeys");
                    else
                        await _redis.StringSetAsync("emailKeys", JsonConvert.SerializeObject(existingEmailsToValidate));
                }
            }

            return await _redis.KeyDeleteAsync(emailValidationCode);
        }
    }
}

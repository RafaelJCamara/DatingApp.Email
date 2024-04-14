using Email.Domain.Constants;
using Email.Domain.Models;
using Email.Infrastructure.Constants;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Email.Infrastructure.Services.Redis
{
    public class EmailValidationService : IEmailValidationService
    {
        private IDatabase _redis;

        public EmailValidationService(IDatabase redis)
        {
            _redis = redis;
        }

        public async Task AddNewEmailToValidate(string emailValidationCode, string username, string userEmail)
        {
            var currentEmailsToValidateKeys = JsonConvert.DeserializeObject<List<string>>((await _redis.StringGetAsync(RedisConstants.EmailValidationListKey)).ToString()) ?? new();
            
            currentEmailsToValidateKeys.Add(emailValidationCode);

            await _redis.StringSetAsync(RedisConstants.EmailValidationListKey, JsonConvert.SerializeObject(currentEmailsToValidateKeys));

            await _redis.StringSetAsync(emailValidationCode, JsonConvert.SerializeObject(new UserEmail
            {
                Username = username,
                Email = userEmail
            }));
        }

        public async Task<IEnumerable<UserEmail>> BlockEmails()
        {
            var existingEmailsToValidate = JsonConvert.DeserializeObject<List<string>>((await _redis.StringGetAsync(RedisConstants.EmailValidationListKey)).ToString());

            var blockedEmails = new List<UserEmail>();

            foreach (var existingEmailToValidate in existingEmailsToValidate ?? Enumerable.Empty<string>())
            {
                var emailToValidate = JsonConvert.DeserializeObject<UserEmail>((await _redis.StringGetAsync(existingEmailToValidate)).ToString());

                if (emailToValidate is not null)
                {
                    var numberOfDaysEmailIsNotValidate = (DateTime.UtcNow - emailToValidate.DateInserted).Days;

                    if (numberOfDaysEmailIsNotValidate > EmailConstants.MaxDaysToValidateEmailOnceRegistered)
                    {
                        blockedEmails.Add(emailToValidate);
                        if (existingEmailsToValidate.Any()) existingEmailsToValidate.Remove(existingEmailToValidate);
                    }
                }
            }
            await _redis.StringSetAsync(RedisConstants.EmailValidationListKey, JsonConvert.SerializeObject(existingEmailsToValidate));

            return blockedEmails;
        }
    }
}

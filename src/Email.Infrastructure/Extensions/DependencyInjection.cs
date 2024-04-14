using DatingApp.Common.Extensions;
using Email.Application.Services.Interfaces;
using Email.Infrastructure.Email;
using Email.Infrastructure.Events.Handlers;
using Email.Infrastructure.Jobs;
using Email.Infrastructure.Services.EmailSending;
using Email.Infrastructure.Services.Redis;
using Email.Infrastructure.Settings;
using Mailjet.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using StackExchange.Redis;
using System.Reflection;

namespace Email.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));

            services
                .AddScoped<IDatabase>(serviceProvider =>
                {
                    var redis = ConnectionMultiplexer.Connect("localhost");
                    return redis.GetDatabase();
                })
                .AddScoped<IEmailService, EmailService>()
                .AddRabbitMQ(configuration, new List<Assembly> { Assembly.GetAssembly(typeof(UserCreatedEventHandler)) })
                .AddQuartz(configure =>
                {
                    var jobKey = new JobKey(nameof(EmailVerificationJob));

                    configure
                        .AddJob<EmailVerificationJob>(jobKey)
                        .AddTrigger(
                            trigger => trigger.ForJob(jobKey).WithSimpleSchedule(
                                schedule => schedule.WithIntervalInHours(24).RepeatForever()));

                    configure.UseMicrosoftDependencyInjectionJobFactory();
                })
                .AddQuartzHostedService(options =>
                {
                    options.WaitForJobsToComplete = true;
                })
                .AddScoped<IEmailSenderService, EmailSenderService>()
                .AddScoped<IEmailValidationService, EmailValidationService>();


            services.AddHttpClient<IMailjetClient, MailjetClient>(client =>
            {
                client.UseBasicAuthentication(configuration["EmailSettings:ApiKey"], configuration["EmailSettings:ApiSecret"]);
            });

            return services;
        }
    }
}

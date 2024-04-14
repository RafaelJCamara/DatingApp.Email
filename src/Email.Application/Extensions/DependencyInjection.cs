using DatingApp.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Email.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatrFromAssemblyContaining(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}

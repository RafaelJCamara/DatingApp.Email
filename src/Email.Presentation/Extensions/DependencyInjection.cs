using DatingApp.Common.Extensions;

namespace Email.Presentation.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                    .AddEndpointsApiExplorer()
                    .AddSwaggerGen()
                    .AddSecurityExtensions(configuration)
                    .AddCommonHelpers()
                    .AddCors()
                    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static WebApplication RunEmailMicroservice(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:4200"));

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();

            return app;
        }
    }
}

using DatingApp.Common.Extensions;
using Email.Application.Extensions;
using Email.Infrastructure.Extensions;
using Email.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddPresentationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices()
    .AddTelemetryExtensions("DatingApp.Email");

var app = builder.Build();

app.RunEmailMicroservice();

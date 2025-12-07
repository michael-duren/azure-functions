using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.AddSingleton<Duren.EmailAutumn.OpenAi>();
builder.Services.AddSingleton(_ =>
{
    var connString =
        Environment.GetEnvironmentVariable("ACS_CONNECTION_STRING")
        ?? throw new InvalidOperationException(
            "ACS_CONNECTION_STRING environment variable is not set."
        );
    return new Azure.Communication.Email.EmailClient(connString);
});

builder
    .Services.AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();


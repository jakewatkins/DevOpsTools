using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

var newRelicApiKey = Environment.GetEnvironmentVariable("NEW_RELIC_LICENSE_KEY");
var newRelicLogUrl = Environment.GetEnvironmentVariable("NEW_RELIC_LOG_URL") ?? "https://log-api.newrelic.com/log/v1";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.NewRelicLogs(newRelicApiKey, newRelicLogUrl)
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog(Log.Logger, dispose: true);
});

builder.Build().Run();

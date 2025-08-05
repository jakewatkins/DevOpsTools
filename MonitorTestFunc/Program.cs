using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Settings.Configuration;
using GuerillaProgrammer;


Console.WriteLine("Starting MonitorTestFunc...");
var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";
var workingDirectory = Environment.CurrentDirectory;

var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(workingDirectory, "settings.json"), false, true)
                .AddJsonFile(Path.Combine(workingDirectory, $"{environment}.settings.json"), true, true)
                .AddJsonFile(Path.Combine(workingDirectory, "local.settings.json"), true, true)
                .AddEnvironmentVariables()
                .Build();

builder.Configuration.AddConfiguration(configuration);




builder.Services.AddLogging(loggingBuilder =>
{
    var options = new ConfigurationReaderOptions { SectionName = "Serilog" };
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration, options)
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();
    loggingBuilder.AddSerilog(Log.Logger, dispose: true);
});

builder.Services.AddTransient<ConfigurationAgent>();

builder.Build().Run();

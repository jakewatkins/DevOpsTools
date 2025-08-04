using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GuerillaProgrammer;

public class Ping
{
    private readonly ILogger<Ping> _logger;
    private readonly IConfiguration _configuration;
    private readonly ConfigurationAgent _configurationAgent;

    public Ping(ILogger<Ping> logger, IConfiguration configuration, ConfigurationAgent configurationAgent)
    {
        _logger = logger;
        _configuration = configuration;
        _configurationAgent = configurationAgent;
    }

    [Function("Ping")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        var isEnabled = _configurationAgent.IsEnabled("PING");
        var count = _configurationAgent.GetCount("PING");
        if (isEnabled == true && count > 0)
        {
            _logger.LogError("Ping - AN ERROR HAS OCCURRED");
            _configurationAgent.UpdateCount("PING", count - 1);
            return new BadRequestObjectResult("Ping function threw an error!");
        }
        else
        {
            _logger.LogInformation("Ping is working");
        }
        return new OkObjectResult("Ping function executed");
    }
}
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GuerillaProgrammer;

public class TickTok
{
    private readonly ILogger<TickTok> _logger;
    private readonly IConfiguration _configuration;
    private readonly ConfigurationAgent _configurationAgent;

    public TickTok(ILogger<TickTok> logger, IConfiguration configuration, ConfigurationAgent configurationAgent)
    {
        _logger = logger;
        _configuration = configuration;
        _configurationAgent = configurationAgent;
    }

    [Function("TickTok")]
    public async Task Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo myTimer)
    {
        var isEnabled = _configurationAgent.IsEnabled("TICKTOK");
        var count = _configurationAgent.GetCount("TICKTOK");
        if (isEnabled == true && count > 0)
        {
            _logger.LogError("TickTok - AN ERROR HAS OCCURRED");
            _configurationAgent.UpdateCount("TICKTOK", count - 1);
        }
        else
        {
            _logger.LogInformation("TickTok is working");
        }
        //return new OkObjectResult("TickTok function executed");
    }
}

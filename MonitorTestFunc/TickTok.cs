using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace GuerillaProgrammer;

public class TickTok
{
    private readonly ILogger<TickTok> _logger;

    public TickTok(ILogger<TickTok> logger)
    {
        _logger = logger;
    }

    [Function("TickTok")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
    {
        var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("TestConfiguration"));
        var tableClient = storageAccount.CreateCloudTableClient();
        var table = tableClient.GetTableReference("TestConfig");
        await table.CreateIfNotExistsAsync();
        var retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>("TEST", "TICKTOK");
        var result = await table.ExecuteAsync(retrieveOperation);
        var entity = result.Result as DynamicTableEntity;
        if (entity != null && entity.Properties.ContainsKey("Enabled") && entity.Properties["Enabled"].BooleanValue == true && entity.Properties.ContainsKey("Count") && entity.Properties["Count"].Int32Value > 0)
        {
            _logger.LogError("TickTok - AN ERROR HAS OCCURRED");
            entity.Properties["Count"].Int32Value--;
            var updateOperation = TableOperation.Replace(entity);
            await table.ExecuteAsync(updateOperation);
        }
        else
        {
            _logger.LogInformation("TickTok is working");
        }
    }
}

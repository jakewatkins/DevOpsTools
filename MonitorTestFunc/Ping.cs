using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace GuerillaProgrammer;

public class Ping
{
    private readonly ILogger<Ping> _logger;

    public Ping(ILogger<Ping> logger)
    {
        _logger = logger;
    }

    [Function("Ping")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("TestConfiguration"));
        var tableClient = storageAccount.CreateCloudTableClient();
        var table = tableClient.GetTableReference("TestConfig");
        await table.CreateIfNotExistsAsync();
        var retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>("TEST", "PING");
        var result = await table.ExecuteAsync(retrieveOperation);
        var entity = result.Result as DynamicTableEntity;
        if (entity != null && entity.Properties.ContainsKey("Enabled") && entity.Properties["Enabled"].BooleanValue == true && entity.Properties.ContainsKey("Count") && entity.Properties["Count"].Int32Value > 0)
        {
            _logger.LogError("Ping - AN ERROR HAS OCCURRED");
            entity.Properties["Count"].Int32Value--;
            var updateOperation = TableOperation.Replace(entity);
            await table.ExecuteAsync(updateOperation);
        }
        else
        {
            _logger.LogInformation("Ping is working");
        }
        return new OkObjectResult("Ping function executed");
    }
}
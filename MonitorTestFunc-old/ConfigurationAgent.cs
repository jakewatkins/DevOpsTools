


using System.Data;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GuerillaProgrammer;

public class ConfigurationAgent
{
    private readonly ILogger<ConfigurationAgent> _logger;
    private readonly IConfiguration _configuration;

    public ConfigurationAgent(ILogger<ConfigurationAgent> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    private DynamicTableEntity GetConfigurationEntity(string functionName)
    {
        var storageAccountConnectionString = _configuration.GetConnectionString("TestConfiguration");
        var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
        var tableClient = storageAccount.CreateCloudTableClient();
        var table = tableClient.GetTableReference("TestConfig");
        table.CreateIfNotExists();

        var retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>("TEST", functionName);
        var result = table.Execute(retrieveOperation);
        return result.Result as DynamicTableEntity;
    }


    public bool IsEnabled(string functionName)
    {
        var entity = GetConfigurationEntity(functionName);

        if (entity != null && entity.Properties.ContainsKey("Enabled") && entity.Properties["Enabled"].BooleanValue == true)
        {
            _logger.LogInformation($"{functionName} is enabled");
            return true;
        }
        else
        {
            _logger.LogInformation($"{functionName} is disabled");
            return false;
        }
    }

    public int GetCount(string functionName)
    {
        var entity = GetConfigurationEntity(functionName);

        if (entity != null && entity.Properties.ContainsKey("Count") && entity.Properties["Count"].Int32Value.HasValue)
        {
            _logger.LogInformation($"{functionName} count is {entity.Properties["Count"].Int32Value}");
            return entity.Properties["Count"].Int32Value.Value;
        }
        else
        {
            _logger.LogInformation($"{functionName} count is not set or invalid");
            return 0;
        }
    }

    public void UpdateCount(string functionName, int count)
    {
        var entity = GetConfigurationEntity(functionName);

        if (entity != null)
        {
            entity.Properties["Count"] = new EntityProperty(count);
            var updateOperation = TableOperation.Replace(entity);
            var storageAccountConnectionString = _configuration.GetConnectionString("TestConfiguration");
            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("TestConfig");
            table.Execute(updateOperation);
            _logger.LogInformation($"{functionName} count updated to {count}");
        }
        else
        {
            _logger.LogWarning($"{functionName} configuration entity not found");
        }
    }
}
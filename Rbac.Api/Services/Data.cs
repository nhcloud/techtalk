using Rbac.Api.Infrastructure;

namespace Rbac.Api.Services;

public class Data : IData
{
    private static readonly string CollectionName = "users";
    private readonly ICosmosDbClientFactory _cosmosDbClientFactory;
    private readonly IAzureStorageClient _storageClientFactory;

    public Data(ICosmosDbClientFactory factory, IAzureStorageClient storage)
    {
        _cosmosDbClientFactory = factory;
        _storageClientFactory = storage;
    }

    public async Task<Dictionary<string, object>> GetAsync(Dictionary<string, object> item)
    {
        var query = $"select c.PartitionKey from c where c.upn='{item["upn"]}'";
        var result = await _cosmosDbClientFactory.GetClient(CollectionName).GetItemsAsync(query, new Dictionary<string, object>());
        return result[0];
    }

    public async Task UpdateAsync(Dictionary<string, object> item)
    {
        await _cosmosDbClientFactory.GetClient(CollectionName).UpsertItemAsync(item);
    }
    public async Task<string> DownloadAsync(string fileName)
    {
        var content = await _storageClientFactory.DownloadAsync(fileName);
        return content;
    }
}
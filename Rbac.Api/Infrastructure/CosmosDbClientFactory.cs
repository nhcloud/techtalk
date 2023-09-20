using Microsoft.Azure.Cosmos;

namespace Rbac.Api.Infrastructure;

public class CosmosDbClientFactory : ICosmosDbClientFactory
{
    private readonly List<string> _collectionNames;
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;

    public CosmosDbClientFactory(string? databaseName, List<string>? collectionNames, CosmosClient cosmosClient)
    {
        _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        _collectionNames = collectionNames ?? throw new ArgumentNullException(nameof(collectionNames));
        _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
    }

    public ICosmosDbClient GetClient(string collectionName)
    {
        if (!_collectionNames.Contains(collectionName))
        {
            throw new ArgumentException($"Unable to find collection: {collectionName}");
        }

        return new CosmosDbClient(_cosmosClient.GetDatabase(_databaseName), collectionName);
    }
}
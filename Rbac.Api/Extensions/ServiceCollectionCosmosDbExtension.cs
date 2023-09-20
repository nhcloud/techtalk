using Rbac.Api.Infrastructure;
using Azure.Identity;
using Microsoft.Azure.Cosmos;

namespace Rbac.Api.Extensions;
public static class ServiceCollectionCosmosDbExtension
{
    public static IServiceCollection AddCosmosDb(this IServiceCollection services, string? connectionString, string? databaseName, List<string>? collectionNames)
    {
        connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        collectionNames = collectionNames ?? throw new ArgumentNullException(nameof(collectionNames));
        CosmosClient cosmosClient;
        if (connectionString.StartsWith("https://"))
        {
            cosmosClient = new CosmosClient(connectionString, new ManagedIdentityCredential());
        }
        else
        {
            cosmosClient = new CosmosClient(connectionString);
        }
        var cosmosDbClientFactory = new CosmosDbClientFactory(databaseName, collectionNames, cosmosClient);

        services.AddSingleton<ICosmosDbClientFactory>(cosmosDbClientFactory);

        return services;
    }
}
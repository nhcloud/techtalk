namespace Rbac.Api.Infrastructure;

public interface ICosmosDbClientFactory
{
    ICosmosDbClient GetClient(string collectionName);
}
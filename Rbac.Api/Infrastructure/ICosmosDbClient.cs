namespace Rbac.Api.Infrastructure;

public interface ICosmosDbClient
{
    Task UpsertItemAsync(Dictionary<string, object> item);
    Task<string?> GetValueAsync(string  query, Dictionary<string, object>? queryParam,string columnName);
    Task<List<Dictionary<string, object>>> GetItemsAsync(string query, Dictionary<string, object> queryParam);
    Task<Dictionary<string, object>> GetItemAsync(string query, Dictionary<string, object> queryParam);
}
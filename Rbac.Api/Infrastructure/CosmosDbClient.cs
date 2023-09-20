using System.Net;
using Microsoft.Azure.Cosmos;

namespace Rbac.Api.Infrastructure;

public class CosmosDbClient : ICosmosDbClient
{
    private readonly Container _container;
    private readonly string[] _ignoreColumns = "_rid,_self,_etag,_attachments,_ts,@odata.id".Split(",");
    public double RequestUnits;


    public CosmosDbClient(Database database, string collectionName)
    {
        _container = database.GetContainer(collectionName);
    }


    public async Task UpsertItemAsync(Dictionary<string, object> item)
    {
        if (!item.ContainsKey("id"))
            item.Add("id", Guid.NewGuid().ToString());
        var partitionKey = new PartitionKey(item["PartitionKey"].ToString());
        var result = await _container.UpsertItemAsync(item, partitionKey);
        RequestUnits += result.RequestCharge;
    }

    public async Task<string?> GetValueAsync(string query, Dictionary<string, object>? queryParam, string columnName)
    {
        try
        {
            var results = await ExecuteAsync(query, queryParam);
            if (results.Count > 0) return results[0][columnName].ToString();
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
        }

        return "";
    }

    public async Task<List<Dictionary<string, object>>> GetItemsAsync(string query, Dictionary<string, object> queryParam)
    {
        try
        {
            return await ExecuteAsync(query, queryParam);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
        }

        return new List<Dictionary<string, object>>();
    }

    public async Task<Dictionary<string, object>> GetItemAsync(string query, Dictionary<string, object> queryParam)
    {
        try
        {
            var results = await ExecuteAsync(query, queryParam);
            if (results.Count > 0) return results[0];
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
        }

        return new Dictionary<string, object>();
    }

    private async Task<List<Dictionary<string, object>>> ExecuteAsync(string query, Dictionary<string, object>? queryParam)
    {
        var results = new List<Dictionary<string, object>>();
        try
        {
            var queryDefinition = new QueryDefinition(query);
            if (queryParam is { Count: > 0 })
            {
                foreach (var key in queryParam.Keys)
                {
                    queryDefinition.WithParameter(key, queryParam[key]);
                }
            }
            var queryResultSetIterator = _container.GetItemQueryIterator<Dictionary<string, object>>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                RequestUnits += currentResultSet.RequestCharge;
                foreach (var result in currentResultSet)
                {
                    foreach (var col in _ignoreColumns)
                        result.Remove(col);
                    results.Add(result);
                }
            }
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            //ignore
        }
        catch (Exception)
        {
            //ignore
        }

        return results;
    }
}
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Scripts;
const string  DatabaseName = "cosmicworks";
string endpoint = "https://learnlive.documents.azure.com:443/";
string key = "ZErlv07Q6EY5MEqxHp13LAfejAlAUvK5oz8pPU0IsnNaQm6s5hi0TBcEuGEf3xR5FZ36cm8sBmzfSj328QKl3Q==";

string endpointMultiWrite = "https://learnlive-mm.documents.azure.com:443/";
string keyMultiWrite = "1NBGAGGHFXBsuZbYv6vWsQbCpoNc3JZarBqObLot2uIWSOGqX6jFV2gziXpHQfL44lrk12UgixQpW35NLPpRzg==";

//LAB Demo

CosmosClientBuilder builder = new(endpoint, key);

using CosmosClient client = builder.Build();
Container container = client.GetContainer(DatabaseName, "products");

string id = $"{Guid.NewGuid()}";
string categoryId = $"{Guid.NewGuid()}";
Product item = new(id, "Polished Bike Frame", categoryId);

var response = await container.CreateItemAsync<Product>(item);

Console.WriteLine($"Status Code:\t{response.StatusCode}");
Console.WriteLine($"Charge (RU):\t{response.RequestCharge:0.00}");

//Modified Demo

await WriteAndRead(endpoint, key, new List<string> { Regions.EastUS });
Console.WriteLine("Please Enter Any key to run Multi-Write Test.");
Console.ReadLine();
await WriteAndRead(endpointMultiWrite, keyMultiWrite, new List<string> { Regions.EastUS });

await SetCustomConflictResolutionPath(endpointMultiWrite, keyMultiWrite, "products2", "/categoryId", "/sortableTimestamp");
await SetResolveConflictUsingFeed(endpointMultiWrite, keyMultiWrite, "products3", "/categoryId");
await SetResolveConfiglictUsingStoredProc(endpointMultiWrite, keyMultiWrite, "products4", "/categoryId");

static async Task WriteAndRead(string endpoint, string key, List<string> regions)
{
    CosmosClientBuilder builder = new(endpoint, key);
    if (regions.Count == 1)
    {
        builder.WithApplicationRegion(regions[0]);
    }
    if (regions.Count > 1)
    {
        builder.WithApplicationPreferredRegions(regions);
    }

    //How to use CosmosClientOptions
    // CosmosClientOptions options = new() { ApplicationPreferredRegions = regions };
    // using CosmosClient client = new(connectionString, options);

    using CosmosClient client = builder.Build();
    Container container = client.GetContainer(DatabaseName, "products");

    var products = GenerateProduct(10);

    var stopwatch = new System.Diagnostics.Stopwatch();
    foreach (var product in products)
    {
        stopwatch.Restart();
        var writeRresponse = await container.CreateItemAsync<Product>(product);
        stopwatch.Stop();
        Console.WriteLine($"Status Code:\t{writeRresponse.StatusCode}; Charge (RU):\t{writeRresponse.RequestCharge:0.00}\t Elapsed:{stopwatch.Elapsed.TotalMilliseconds}");
    }

    foreach (var product in products)
    {
        stopwatch.Restart();
        var readResponse = await container.ReadItemAsync<Product>(product.id, new PartitionKey(product.categoryId));
        stopwatch.Stop();
        Console.WriteLine($"Status Code:\t{readResponse.StatusCode}; Charge (RU):\t{readResponse.RequestCharge:0.00}\t Elapsed:{stopwatch.Elapsed.TotalMilliseconds}");
    }
}

static async Task<bool> SetCustomConflictResolutionPath(string endpoint, string key, string containerName, string partitionKey, string conflictResolutionPath)
{
    CosmosClientBuilder builder = new(endpoint, key);
    using CosmosClient client = builder.Build();
    Database database = client.GetDatabase(DatabaseName);
    ContainerProperties properties = new(containerName, partitionKey)
    {
        ConflictResolutionPolicy = new ConflictResolutionPolicy()
        {
            Mode = ConflictResolutionMode.LastWriterWins,
            ResolutionPath = conflictResolutionPath,//"/metadata/sortableTimestamp"
        }
    };
    Container container = await database.CreateContainerIfNotExistsAsync(properties);
    return true;
}

static async Task<bool> SetResolveConflictUsingFeed(string endpoint, string key, string containerName, string partitionKey)
{
    CosmosClientBuilder builder = new(endpoint, key);
    using CosmosClient client = builder.Build();
    Database database = client.GetDatabase(DatabaseName);
    ContainerProperties properties = new(containerName, partitionKey)
    {
        ConflictResolutionPolicy = new ConflictResolutionPolicy()
        {
            Mode = ConflictResolutionMode.Custom
        }
    };
    Container container = await database.CreateContainerIfNotExistsAsync(properties);
    return true;
}

static async Task<bool> SetResolveConfiglictUsingStoredProc(string endpoint, string key, string containerName, string partitionKey)
{
    const string sprocName = "resolveConflicts";
    CosmosClientBuilder builder = new(endpoint, key);
    using CosmosClient client = builder.Build();
    Database database = client.GetDatabase(DatabaseName);

    ContainerProperties properties = new(containerName, partitionKey)
    {
        ConflictResolutionPolicy = new ConflictResolutionPolicy()
        {
            Mode = ConflictResolutionMode.Custom,
            ResolutionProcedure = $"dbs/{DatabaseName}/colls/{containerName}/sprocs/{sprocName}"
        }
    };
    Container container = await database.CreateContainerIfNotExistsAsync(properties);
    StoredProcedureProperties spProperties = new (sprocName, File.ReadAllText(@"code.js"));
    await container.Scripts.CreateStoredProcedureAsync(spProperties); 
    return true;
}

static List<Product> GenerateProduct(uint numberOfItems)
{
    var products = new List<Product>();
    var random = new Random();
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    for (var i = 0; i < numberOfItems; i++)
    {
        products.Add(new($"{Guid.NewGuid()}", new string(Enumerable.Repeat(chars, 8)
          .Select(s => s[random.Next(s.Length)]).ToArray()), "Multi-Write-Demo"/*$"{Guid.NewGuid()}"*/));
    }
    return products;
}
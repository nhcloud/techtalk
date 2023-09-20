using Azure.Identity;
using Azure.Storage.Blobs;

namespace Rbac.Api.Infrastructure;

public class AzureStorageClient : IAzureStorageClient
{
    private readonly BlobContainerClient _blobContainerClient;

    public AzureStorageClient(string connectionString, string containerName)
    {
        if (connectionString.StartsWith("https://"))
        {
            _blobContainerClient = new BlobServiceClient(new Uri(connectionString + ".blob.core.windows.net"), new DefaultAzureCredential()).GetBlobContainerClient(containerName);
        }
        else
        {
            _blobContainerClient = new BlobServiceClient(connectionString).GetBlobContainerClient(containerName);
        }
    }

    public async Task<string> DownloadAsync(string fileName)
    {
        var content = await _blobContainerClient.GetBlobClient(fileName).DownloadContentAsync();
        return content.Value.Content.ToString();
    }
}
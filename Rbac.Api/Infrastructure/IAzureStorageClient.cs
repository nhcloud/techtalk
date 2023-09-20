namespace Rbac.Api.Infrastructure;

public interface IAzureStorageClient
{
    Task<string> DownloadAsync(string fileName);
}
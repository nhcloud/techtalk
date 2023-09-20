using Rbac.Api.Infrastructure;

namespace Rbac.Api.Extensions;

public static class ServiceCollectionAzureStorageExtension
{
    public static IServiceCollection AddAzureStorage(this IServiceCollection services, string? connectionString, string? containerName)
    {
        connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        containerName = containerName ?? throw new ArgumentNullException(nameof(containerName));
        var storageClient = new AzureStorageClient(connectionString, containerName);


        services.AddSingleton<IAzureStorageClient>(storageClient);

        return services;
    }
}
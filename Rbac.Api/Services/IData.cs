namespace Rbac.Api.Services;

public interface IData
{
    Task<Dictionary<string, object>> GetAsync(Dictionary<string, object> item);
    Task UpdateAsync(Dictionary<string, object> item);
    Task<string> DownloadAsync(string fileName);
}
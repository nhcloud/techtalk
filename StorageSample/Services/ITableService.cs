using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace StorageSample.Services
{
    public interface ITableService
    {
        Task<bool> AddOrUpdateAsync(DynamicTableEntity data);
        Task<DynamicTableEntity> GetAsync(string partitionKey, string rowKey);
    }
}

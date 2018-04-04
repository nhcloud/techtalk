using System.Threading.Tasks;
using Microsoft.Azure.KeyVault.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StorageSample.Helpers;
using StorageSample.Models;

namespace StorageSample.Services
{
    public class TableService:ITableService
    {
        private static readonly CloudTable Table;
        private static readonly IKey KeyVaultKey;
        static TableService()
        {
            var storageConnection = Utility.GetSecret(AppSettings.StorageConnection);
            var tableClient = CloudStorageAccount.Parse(storageConnection).CreateCloudTableClient();
            Table = tableClient.GetTableReference(AppSettings.TableName);
            Table.CreateIfNotExists();
            KeyVaultKey = Utility.GetKey(AppSettings.KeyUri);
        }
        private static TableRequestOptions GetTableRequestOption()
        {
            var tableRequestOption = new TableRequestOptions
            {
                EncryptionPolicy = new TableEncryptionPolicy(KeyVaultKey, null)
            };
            return tableRequestOption;
        }
        public async Task<bool> AddOrUpdateAsync(DynamicTableEntity data)
        {
            var action = TableOperation.InsertOrReplace(data);
            await Table.ExecuteAsync(action, GetTableRequestOption(), null);
            return true;
        }

        public async Task<DynamicTableEntity> GetAsync(string partitionKey, string rowKey)
        {
            var action = TableOperation.Retrieve<DynamicTableEntity>(partitionKey, rowKey);
            var response = await Table.ExecuteAsync(action, GetTableRequestOption(), null);
            var result = response.Result as DynamicTableEntity;
            return result;
        }
    }
}

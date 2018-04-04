using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using StorageSample.Services;

namespace StorageSample.Controllers
{
    [Route("api/[controller]")]
    public class StorageController : Controller
    {
        [HttpGet("blob/{id}")]
        public string GetBlob(string id)
        {
            var b = new BlobService();
            return b.ReadAllText(id);
        }

        [HttpPost("blob")]
        public string Post([FromBody]string value)
        {
            var fileName = Guid.NewGuid() + ".txt";
            var b = new BlobService();
            b.WriteAllText($"bcc29-sample text:{DateTime.UtcNow}", fileName);
            return fileName;
        }
        [HttpGet("table/{partitionkey}/{rowkey}")]
        public async Task<string> GetTable(string partitionkey, string rowkey)
        {
            var t = new TableService();
            var result = await t.GetAsync(partitionkey, rowkey);
            return result["Value"].StringValue;
        }

        [HttpPost("table")]
        public async Task<string> PostTable([FromBody]string value)
        {
            var map = new DynamicTableEntity
            {
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = Guid.NewGuid().ToString()
            };
            IDictionary<string, EntityProperty> properties = new ConcurrentDictionary<string, EntityProperty>();
            properties.Add("Value", new EntityProperty($"bcc29-sample text:{DateTime.UtcNow}"));
            var t = new TableService();
            map.Properties = properties;
            await t.AddOrUpdateAsync(map);
            return $"{map.PartitionKey}/{map.RowKey}";
        }
    }
}

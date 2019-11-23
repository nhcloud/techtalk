using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DependencyInjection.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DependencyInjection.Functions
{
    public class StorageOp
    {
        //private readonly IStorage _storage;
        //public StorageOp(IStorage storage)
        //{
        //    _storage = storage;
        //}

        private readonly IEnumerable<IStorage> _storage;
        public StorageOp(IEnumerable<IStorage> storage)
        {
            _storage = storage;
        }

        [FunctionName("StorageOp")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            if (name == null)
            {
                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

            foreach (var store in _storage)
                await store.Write(name);
            return (ActionResult)new OkObjectResult($"Message {name} added to all provider");

            //return (ActionResult)new OkObjectResult(await _storage.Write(name));
        }
    }
}

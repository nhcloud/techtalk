using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleFunction
{
    public static class SimpleFunc
    {
        [FunctionName("SimpleFunc")]
        public static async Task<IActionResult> Run(
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
            var storage = new FileStorage();
            await storage.Write(name);
            return (ActionResult)new OkObjectResult($"Message {name} added.");
        }
    }
    public class FileStorage
    {
        private readonly static string Store = @"c:\temp\simpledata.json";
        static FileStorage()
        {
            if (!File.Exists(Store))
            {
                File.WriteAllText(Store, "");
            }
        }
        public string Read(string msg)
        {
            return $"Hello, {msg}. I'm a sample message.";
        }
        public async Task<List<string>> ReadAll()
        {

            var result = await Task.Run(() =>
            {
                var msg = File.ReadAllText(Store);
                return JsonConvert.DeserializeObject<List<string>>(msg);
            });
            return result ?? new List<string>();
        }

        public async Task<string> Write(string msg)
        {
            var current = await ReadAll();
            current.Add(msg);
            File.WriteAllText(Store, JsonConvert.SerializeObject(current));
            return msg + " stored!";
        }
    }
}

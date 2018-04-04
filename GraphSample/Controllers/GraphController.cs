using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphSample.Models;
using GraphSample.Models.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.WindowsAzure.Storage.Table;


namespace GraphSample.Controllers
{
    [Route("api/[controller]")]
    public class GraphController : Controller
    {
        [HttpGet("")]
        public async Task<string> GetBlob(string pathAndQuery)
        {
            try
            {
                var graphToken = ServiceHelper.Caching.Get("graphTokens") as TokenResponse;
                var config = TenantAuthConfig.GetDefaultConfig();
                var client = new OAuth2Client(config);
                return await client.GetData($"{config.GraphUri}/{pathAndQuery}", null, graphToken.Token);
            }
            catch (Exception e)
            {
                return "Error";
            }
        }

        [HttpPost("")]
        public string Post([FromBody]string value)
        {
            return "";
        }
    }
}

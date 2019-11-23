using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DurableFunctions
{
    public static class Function1
    {
        [FunctionName("Durable")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("Durable_Hello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("Durable_Hello", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("Durable_Hello", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("Durable_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("Durable_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Durable", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        private const string Timeout = "timeout";
        private const string RetryInterval = "retryInterval";
        [FunctionName("Durable_HttpSyncStart")]
        public static async Task<HttpResponseMessage> HttpSyncStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            dynamic eventData = await req.Content.ReadAsAsync<object>();
            string instanceId = await starter.StartNewAsync("DurableFunction", eventData);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            TimeSpan timeout = GetTimeSpan(req, Timeout);
            TimeSpan retryInterval = GetTimeSpan(req, RetryInterval);

            return await starter.WaitForCompletionOrCreateCheckStatusResponseAsync(
                req,
                instanceId,
                timeout,
                retryInterval);
        }
        private static TimeSpan GetTimeSpan(HttpRequestMessage request, string queryParameterName)
        {
            string queryParameterStringValue = request.RequestUri.ParseQueryString()[queryParameterName];
            if (string.IsNullOrEmpty(queryParameterStringValue))
            {
                queryParameterStringValue = "30";
            }

            return TimeSpan.FromSeconds(double.Parse(queryParameterStringValue));
        }
    }
}
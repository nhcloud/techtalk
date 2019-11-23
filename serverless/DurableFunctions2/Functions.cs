using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DurableFunctions2
{
    public static class Functions
    {
        [FunctionName("EventHubTriggerCSharp")]
        public static async Task Run(
    [EventHubTrigger("device-sensor-events")] EventData eventData,
    [OrchestrationClient] IDurableOrchestrationClient entityClient)
        {
            var metricType = (string)eventData.Properties["metric"];
            var delta = BitConverter.ToInt32(eventData.Body, eventData.Body.Offset);

            // The "Counter/{metricType}" entity is created on-demand.
            var entityId = new EntityId("Counter", metricType);
            await entityClient.SignalEntityAsync(entityId, "add", delta);
        }
    }
    public class Counter
    {
        [JsonProperty("value")]
        public int CurrentValue { get; set; }

        public void Add(int amount) => this.CurrentValue += amount;

        public void Reset() => this.CurrentValue = 0;

        public int Get() => this.CurrentValue;

        [FunctionName(nameof(Counter))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Counter>();
    }
}
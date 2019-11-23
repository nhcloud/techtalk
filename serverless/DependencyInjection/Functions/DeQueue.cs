using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace DependencyInjection.Functions
{
    public static class DeQueue
    {
        [FunctionName("DeQueue")]
        public static void Run([QueueTrigger("samplequeue", Connection = "")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}

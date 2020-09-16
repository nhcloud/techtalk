using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace SignalRWeb.Models
{
    public class SampleData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
    public static class Helper
    {
        public static async Task<SampleData[]> GetDataAsync()
        {
            return await Task.Run(() =>
            {
                var content = File.ReadAllText("C:/Users/udaia/source/repos/SignalRDemo/data.json");
                return JsonConvert.DeserializeObject<SampleData[]>(content);
            });
        }
    }
}

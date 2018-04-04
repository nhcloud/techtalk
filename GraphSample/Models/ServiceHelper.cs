
using Microsoft.Extensions.Caching.Memory;

namespace GraphSample.Models
{
    public static class ServiceHelper
    {
        public static HttpUtility HttpUtility { get; set; }
        public static IMemoryCache Caching { get; set; }
    }
}

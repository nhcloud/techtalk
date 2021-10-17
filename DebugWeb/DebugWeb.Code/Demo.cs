using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DebugWeb.Code
{
    public class Demo : IDemo
    {
        public async Task<string> DownloadUrlAsync(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
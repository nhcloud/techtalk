using System.Threading.Tasks;

namespace DebugWeb.Code
{
    public interface IDemo
    {
        Task<string> DownloadUrlAsync(string url);
    }
}
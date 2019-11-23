using System.Collections.Generic;
using System.Threading.Tasks;

namespace DependencyInjection.Services
{
    public interface IStorage
    {
        string Read(string msg);
        Task<string> Write(string msg);
        Task<List<string>> ReadAll();
    }
}

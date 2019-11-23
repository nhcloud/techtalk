using System.Collections.Generic;
using System.Threading.Tasks;

namespace DependencyInjection.Services
{
    public class MemoryStorage : IStorage
    {
        private static List<string> InMemoryStore = new List<string>();
        public string Read(string msg)
        {
            return $"Hello, {msg}. I'm a sample message.";
        }
        public async Task<List<string>> ReadAll()
        {
            return await Task.Run(() =>
            {
                return InMemoryStore;
            });
        }

        public async Task<string> Write(string msg)
        {
            return await Task.Run(() =>
            {
                InMemoryStore.Add(msg);
                return msg + " stored!";
            });
        }
    }
}

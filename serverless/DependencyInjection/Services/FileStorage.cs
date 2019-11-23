using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace DependencyInjection.Services
{
    public class FileStorage : IStorage
    {
        private readonly static string Store = @"c:\temp\data.json";
        static FileStorage()
        {
            if (!File.Exists(Store))
            {
                File.WriteAllText(Store, "");
            }
        }
        public string Read(string msg)
        {
            return $"Hello, {msg}. I'm a sample message.";
        }
        public async Task<List<string>> ReadAll()
        {

            var result = await Task.Run(() =>
             {
                 var msg = File.ReadAllText(Store);
                 return JsonConvert.DeserializeObject<List<string>>(msg);
             });
            return result ?? new List<string>();
        }

        public async Task<string> Write(string msg)
        {
            var current = await ReadAll();
            current.Add(msg);
            File.WriteAllText(Store, JsonConvert.SerializeObject(current));
            return msg + " stored!";
        }
    }
}

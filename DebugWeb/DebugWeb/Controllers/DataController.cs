using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace DebugWeb.Controllers
{
    public class DataController : ApiController
    {
        // GET: api/DataApi/5
        public async Task<string> Get([FromUri] string url)
        {
            string result;
            result= await ExecSequential(url);
            //result= await Task.Run(() => ExecParallel(url));
            //result = await ExecTasks(url);
            return result;
        }

        private static async Task<string> ExecSequential(string url)
        {
            var items = url.Split(',');
            var results = new List<string>();
            var api = new DebugWeb.Code.Demo();
            foreach (var item in items)
            {
                results.Add(await api.DownloadUrlAsync(item));
            }
            return string.Join("===>", results);
        }
        private async static Task<string> ExecTasks(string url)
        {
            var items = url.Split(',');
            var tasks = new List<Task>();
            var results = new List<string>();
            var api = new DebugWeb.Code.Demo();
           
            foreach (var item in items)
            {
                tasks.Add( Task.Run(async () => results.Add(await api.DownloadUrlAsync(item))));
            }

            Task.WaitAll(tasks.ToArray());
            return string.Join("===>", results);
        }
        private async static Task<string> ExecParallel(string url)
        {
            var items = url.Split(',');
            var actions = new List<Action>();
            var results = new List<string>();
            var api = new DebugWeb.Code.Demo();
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            foreach (var item in items)
            {
                actions.Add(async () =>
                    {
                        results.Add(await api.DownloadUrlAsync(item));
                    }
                );
            }
            //Parallel.Invoke(actions.ToArray());
            Parallel.ForEach(actions, options, action => { action(); });
            return string.Join("===>", results);
        }
    }
}
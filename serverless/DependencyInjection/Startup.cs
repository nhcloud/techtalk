using DependencyInjection.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Serilog;
using System;

[assembly: FunctionsStartup(typeof(DependencyInjection.Startup))]

namespace DependencyInjection
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddScoped<IStorage, FileStorage>();
            builder.Services.AddScoped<IStorage, MemoryStorage>();
            builder.Services.AddScoped<IStorage, FileStorage>();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient("githubapi", c =>
            {
                // Set the base address
                c.BaseAddress = new Uri("https://api.github.com/");

                // Set default headers for the client
                c.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.github.v3+json");
                c.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "AzureFunctions-HttpClient-Sample");
            });
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(@"c:\temp\functionapp.log")
                .CreateLogger();
            builder.Services.AddLogging(b =>
            {
                b.AddSerilog(dispose: true);
            });
        }
    }
}
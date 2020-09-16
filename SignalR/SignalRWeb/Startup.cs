using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalRWeb.Hubs;

namespace SignalRWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var options = new ServiceOptions
            //{
            //    ConnectionString = Configuration.GetSection("AppSettings")["AzureSignalRConnectionString"]
            //};
            services.AddControllersWithViews();
            services.AddMvc();
            services.AddSignalR()
                .AddAzureSignalR(Configuration.GetSection("AppSettings")["AzureSignalRConnectionString"])
                //.AddAzureSignalR(options => options.Endpoints = new ServiceEndpoint[]
                //{
                //    new ServiceEndpoint("<connection_string1>", EndpointType.Primary, "region1"),
                //    new ServiceEndpoint("<connection_string2>", EndpointType.Secondary, "region2"),
                //})
                .AddMessagePackProtocol();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseRouting();
            app.UseFileServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseAzureSignalR(routes =>
            {
                routes.MapHub<ContentHub>("/contenthub");
            });
        }
    }
}
using Rbac.Api.Extensions;
using Rbac.Api.Filters;
using Rbac.Api.Services;
using Azure.Identity;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Rbac.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var appiOptions = new ApplicationInsightsServiceOptions
        {
            ConnectionString = Configuration.GetSection("ApplicationInsights")["ConnectionString"]
            //ConnectionString = "InstrumentationKey=00000000-0000-0000-0000-000000000000;IngestionEndpoint=https://appi-udai.applicationinsights.azure.com/"

        };
        services.Configure<TelemetryConfiguration>(config =>
        {
            var credential = new DefaultAzureCredential();
            config.SetAzureTokenCredential(credential);
        });
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IAppConfiguration, AppConfiguration>();
        services.AddScoped<IData, Data>();
        services.AddSingleton(Configuration);
        services.AddApplicationInsightsTelemetry(appiOptions);
        services.AddApplicationInsightsKubernetesEnricher();
        services.AddMvc();

        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(ValidateRequestFilter));
            options.Filters.Add(typeof(ValidateDeviceFilter));
        }).AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        var configuration = Configuration.GetSection("AppSettings");
        services.AddCosmosDb(configuration["CosmosDbConnectionString"], configuration["CosmosDbName"], configuration["CollectionNames"]?.Split(',').ToList());
        services.AddAzureStorage(configuration["StorageConnectionString"], configuration["StorageContainerName"]);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // must be lower case
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, new string[] { } }
            });
        });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ContextManager.AppConfiguration = new AppConfiguration(Configuration);

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers();
        });
    }
}
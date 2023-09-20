using System.Text;
using Rbac.Api;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore;

var appConfiguration = ConfigurationFromFile();

try
{
    var host = BuildWebHost(appConfiguration, args);
    host.Run();
    return 0;
}
catch (Exception)
{
    return 1;
}

IWebHost BuildWebHost(IConfiguration configuration, string[] args)
{
    return WebHost.CreateDefaultBuilder(args)
        .CaptureStartupErrors(false)
        .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
        .UseStartup<Startup>()
        .Build();
}
IConfiguration ConfigurationFromFile()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddEnvironmentVariables();

    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
    {
        builder.AddJsonFile("appsettings.Development.json");
    }

    var config = builder.Build();
    var appSettings = config.GetSection("AppSettings");
    var secretIdentifier = appSettings["SecretIdentifier"];
    if (string.IsNullOrEmpty(secretIdentifier))
    {
        secretIdentifier = Environment.GetEnvironmentVariable("AppSettingsSecretUri");
        if (string.IsNullOrEmpty(secretIdentifier)) return config;

    }
    var (secretHost, secretName, secretVersion) = SliceIdentifier(secretIdentifier);
    var sc = new SecretClient(new Uri(secretHost), new DefaultAzureCredential());
    var stream = new MemoryStream(Encoding.UTF8.GetBytes(sc.GetSecret(secretName, secretVersion).Value.Value));
    builder.AddConfiguration(ConfigurationFromStream(stream));
    return builder.Build();
}
IConfiguration ConfigurationFromStream(Stream stream)
{
    var builder = new ConfigurationBuilder()
        .AddJsonStream(stream)
        .AddEnvironmentVariables();
    return builder.Build();
}
(string Host, string Name, string? Version) SliceIdentifier(string identifier)
{
    var parts = identifier.TrimEnd('/').Replace("/secrets/", "/").Replace("https://", "").Split('/');
    return ($"https://{parts[0]}/", parts[1], parts.Length > 2 ? parts[2] : null);
}
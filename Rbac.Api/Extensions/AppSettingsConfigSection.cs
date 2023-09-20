namespace Rbac.Api.Extensions;
public interface IAppConfiguration
{
    IConfigurationSection GetSection(string section);
}

public class AppConfiguration : IAppConfiguration
{
    private static IConfiguration _configuration;

    public AppConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IConfigurationSection GetSection(string section)
    {
        return _configuration.GetSection(section);
    }
}
public class ContextManager
{
    public static IAppConfiguration AppConfiguration { get; set; }
}
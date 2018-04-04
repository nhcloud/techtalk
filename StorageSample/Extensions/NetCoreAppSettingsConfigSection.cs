using Microsoft.Extensions.Configuration;
using StorageSample.Models;

namespace StorageSample.Extensions
{
    public class NetCoreAppSettingsConfigSection : IAppSettingsConfigSection
    {
        private static IConfigurationSection _section;
        public NetCoreAppSettingsConfigSection(IConfigurationSection appSettingsSection)
        {
            _section = appSettingsSection;
        }
        public string Get(string key)
        {
            return _section[key];
        }
    }
}

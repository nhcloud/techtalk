namespace StorageSample.Models
{
    public class AppSettings
    {

        private static readonly IAppSettingsConfigSection Section = AppConfig.ConfigSection;

        public static readonly string StorageConnection = Section.Get("StorageConnection");
        public static readonly string ContainerName = Section.Get("ContainerName");
        public static readonly string TableName = Section.Get("TableName");
        public static readonly string KeyUri = Section.Get("KeyUri");
    }
}
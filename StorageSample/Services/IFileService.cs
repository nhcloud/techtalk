namespace StorageSample.Services
{
    public interface IFileService
    {
        void WriteAllText(string path, string contents);
        bool Exists(string path);
        long Length(string path);
        string ReadAllText(string path);
    }
}

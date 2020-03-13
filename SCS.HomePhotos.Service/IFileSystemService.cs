namespace SCS.HomePhotos.Service
{
    public interface IFileSystemService
    {
        void CreateDirectory(string path);
        string GetChecksum(string filePath);

        long GetFileSize(string filePath);
        ImageInfo GetInfo(string filePath);
    }
}
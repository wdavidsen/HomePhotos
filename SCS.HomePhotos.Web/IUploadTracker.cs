namespace SCS.HomePhotos.Web
{
    public interface IUploadTracker
    {
        void AddUpload(string userName, string file);
        void Clear();
        int GetUploadCount(string userName);
        void RemoveUpload(string file);
        bool IsProcessingDone(string userName);
    }
}
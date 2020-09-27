namespace SCS.HomePhotos.Web.Hubs
{
    public interface IClientMessageSender
    {
        void OnIndexCompleted();
        void OnIndexStarted();
    }
}
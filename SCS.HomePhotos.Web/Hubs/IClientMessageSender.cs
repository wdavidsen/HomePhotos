using SCS.HomePhotos.Service.Workers;

namespace SCS.HomePhotos.Web.Hubs
{
    public interface IClientMessageSender
    {
        void OnIndexCompleted();
        void OnIndexStarted();

        void OnItemProcessed(TaskCompleteInfo info);
    }
}
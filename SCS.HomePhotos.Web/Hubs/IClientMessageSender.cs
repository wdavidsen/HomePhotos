using SCS.HomePhotos.Service.Workers;

namespace SCS.HomePhotos.Web.Hubs
{
    /// <summary>
    /// Client push notification sender.
    /// </summary>
    public interface IClientMessageSender
    {
        /// <summary>
        /// Called when photo indeing has completed.
        /// </summary>
        void OnIndexCompleted();

        /// <summary>
        /// Called when photo indexing is started.
        /// </summary>
        void OnIndexStarted();

        /// <summary>
        /// Called when a notification type is processed.
        /// </summary>
        /// <param name="info">The notification information.</param>
        void OnItemProcessed(TaskCompleteInfo info);

        /// <summary>
        /// Called when photo indexing fails.
        /// </summary>
        void OnIndexFailed();
    }
}
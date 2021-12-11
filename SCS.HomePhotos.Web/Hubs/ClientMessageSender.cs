using Microsoft.AspNetCore.SignalR;
using SCS.HomePhotos.Service.Workers;
using System;
using System.Threading;

namespace SCS.HomePhotos.Web.Hubs
{
    /// <summary>
    /// Client push notification sender.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Web.Hubs.IClientMessageSender" />
    public class ClientMessageSender : IClientMessageSender
    {
        private readonly IHubContext<NotifcationHub, INotifcationHub> _notificationHub;
        private readonly IUploadTracker _uploadTracker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMessageSender"/> class.
        /// </summary>
        /// <param name="indexEvents">The index events.</param>
        /// <param name="queueEvents">The queue events.</param>
        /// <param name="notificationHub">The notification hub.</param>
        /// <param name="uploadTracker">The upload tracker.</param>
        public ClientMessageSender(IIndexEvents indexEvents, IQueueEvents queueEvents, IHubContext<NotifcationHub, INotifcationHub> notificationHub,
            IUploadTracker uploadTracker)
        {
            indexEvents.IndexStarted = OnIndexStarted;
            indexEvents.IndexCompleted = OnIndexCompleted;
            indexEvents.IndexFailed = OnIndexFailed;
            queueEvents.ItemProcessed = OnItemProcessed;

            _notificationHub = notificationHub;
            _uploadTracker = uploadTracker;
        }

        /// <summary>
        /// Called when photo indexing is started.
        /// </summary>
        public void OnIndexStarted()
        {
            _notificationHub.Clients.All.SendAdminsMessage("info", "Photo indexing started");
        }

        /// <summary>
        /// Called when photo indeing has completed.
        /// </summary>
        public void OnIndexCompleted()
        {
            _notificationHub.Clients.All.SendAdminsMessage("info", "Photo indexing completed");
        }

        /// <summary>
        /// Called when photo indexing fails.
        /// </summary>
        public void OnIndexFailed()
        {
            _notificationHub.Clients.All.SendAdminsMessage("error", "Photo indexing failed");
        }

        /// <summary>
        /// Called when a notification type is processed.
        /// </summary>
        /// <param name="info">The notification information.</param>
        public void OnItemProcessed(TaskCompleteInfo info)
        {
            switch (info.Type)
            {
                case TaskType.ClearCache:
                    _notificationHub.Clients.All.SendAdminsMessage(
                        info.Success.Value ? "success" : "error",
                        info.Success.Value ? "Photo cache cleared" : "Failed to clear photo cache");
                    break;

                case TaskType.ProcessMobilePhoto:
                    _uploadTracker.RemoveUpload((string)info.Data);

                    var timer = new Timer((state) => {

                        if (_uploadTracker.IsProcessingDone(info.ContextUserName))
                        {
                            var uploadCount = _uploadTracker.GetUploadCount(info.ContextUserName);
                            _notificationHub.Clients.All.SendEveryoneMessage("info", $"{info.ContextUserName} uploaded {uploadCount} photos");
                        }
                    }, null, 1000 * 2, 0);

                    break;
            }
        }
    }
}

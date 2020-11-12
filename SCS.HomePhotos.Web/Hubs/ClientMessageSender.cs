using Microsoft.AspNetCore.SignalR;
using SCS.HomePhotos.Service.Workers;
using System;
using System.Threading;

namespace SCS.HomePhotos.Web.Hubs
{
    public class ClientMessageSender : IClientMessageSender
    {
        private readonly IHubContext<NotifcationHub, INotifcationHub> _notificationHub;
        private readonly IUploadTracker _uploadTracker;
        
        public ClientMessageSender(IIndexEvents indexEvents, IQueueEvents queueEvents, IHubContext<NotifcationHub, INotifcationHub> notificationHub,
            IUploadTracker uploadTracker)
        {
            indexEvents.IndexStarted = OnIndexStarted;
            indexEvents.IndexCompleted = OnIndexCompleted;
            queueEvents.ItemProcessed = OnItemProcessed;

            _notificationHub = notificationHub;
            _uploadTracker = uploadTracker;
        }

        public void OnIndexStarted()
        {
            _notificationHub.Clients.All.SendAdminsMessage("info", "Photo indexing started");
        }

        public void OnIndexCompleted()
        {
            _notificationHub.Clients.All.SendAdminsMessage("info", "Photo indexing completed");
        }

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

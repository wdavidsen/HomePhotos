using Microsoft.AspNetCore.SignalR;
using SCS.HomePhotos.Service.Workers;

namespace SCS.HomePhotos.Web.Hubs
{
    public class ClientMessageSender : IClientMessageSender
    {
        private readonly IHubContext<NotifcationHub, INotifcationHub> _notificationHub;

        public ClientMessageSender(IIndexEvents indexEvents, IHubContext<NotifcationHub, INotifcationHub> notificationHub)
        {
            indexEvents.IndexStarted = OnIndexStarted;
            indexEvents.IndexCompleted = OnIndexCompleted;

            _notificationHub = notificationHub;
        }

        public void OnIndexStarted()
        {
            _notificationHub.Clients.All.SendAdminsMessage("info", "Photo indexing started");
        }

        public void OnIndexCompleted()
        {
            _notificationHub.Clients.All.SendAdminsMessage("info", "Photo indexing completed");
        }
    }
}

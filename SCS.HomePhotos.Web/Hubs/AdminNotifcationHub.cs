using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Hubs
{
    public class AdminNotifcationHub : Hub<IAdminNotifcationHub>, IAdminNotifcationHub
    {
        public AdminNotifcationHub()
        {
        }

        [HubMethodName("SendMessage")]
        public async Task SendMessage(string message)
        {
            await Clients.Groups("Admins").SendMessage(message);
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Service.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Hubs
{
    public class NotifcationHub : Hub<INotifcationHub>, INotifcationHub
    {
        private readonly IAccountService _accountService;

        public NotifcationHub(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HubMethodName("SendAdminsMessage")]
        public async Task SendAdminsMessage(string type, string message)
        {
            var users = await GetAdmins();

            if (users.Count() > 0)
            {
                await Clients
                    .Users(users.Select(e => e.UserName).ToArray())
                    .SendAdminsMessage(type, message);
            }
        }

        [HubMethodName("SendEveryoneMessage")]
        public async Task SendEveryoneMessage(string type, string message)
        {
            await Clients.All.SendAdminsMessage(type, message);
        }

        private async Task<IEnumerable<User>> GetAdmins()
        {
            var admins = await _accountService.GetUsers(RoleType.Admin);
           
            return admins;
        }
    }
}

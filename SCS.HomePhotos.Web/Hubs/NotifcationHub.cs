using Microsoft.AspNetCore.SignalR;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Hubs
{
    /// <summary>
    /// Notification hub for client push notifications.
    /// </summary>
    /// <seealso cref="INotifcationHub" />
    public class NotifcationHub : Hub<INotifcationHub>, INotifcationHub
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifcationHub"/> class.
        /// </summary>
        /// <param name="accountService">The account service.</param>
        public NotifcationHub(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Sends the admins a push notification.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
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

        /// <summary>
        /// Sends everyone a push notification message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        [HubMethodName("SendEveryoneMessage")]
        public async Task SendEveryoneMessage(string type, string message)
        {
            await Clients.All.SendAdminsMessage(type, message);
        }

        /// <summary>
        /// Gets the admin users.
        /// </summary>
        /// <returns>A list of users.</returns>
        private async Task<IEnumerable<User>> GetAdmins()
        {
            var admins = await _accountService.GetUsers(RoleType.Admin);
           
            return admins;
        }
    }
}

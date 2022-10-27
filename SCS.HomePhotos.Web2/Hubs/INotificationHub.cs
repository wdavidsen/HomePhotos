using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Hubs
{
    /// <summary>
    /// Push notification hub.
    /// </summary>
    public interface INotifcationHub
    {
        /// <summary>
        /// Sends the admins a push notification.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        /// <returns>A void task.</returns>
        Task SendAdminsMessage(string type, string message);

        /// <summary>
        /// Sends everyone a push notification message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        /// <returns>A void task.</returns>
        Task SendEveryoneMessage(string type, string message);
    }
}

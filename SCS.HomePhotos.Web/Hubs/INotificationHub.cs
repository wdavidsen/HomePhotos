using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Hubs
{
    public interface INotifcationHub
    {
        Task SendAdminsMessage(string type, string message);

        Task SendEveryoneMessage(string type, string message);
    }
}

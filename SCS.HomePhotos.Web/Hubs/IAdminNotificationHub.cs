using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Hubs
{
    public interface IAdminNotifcationHub
    {
        Task SendMessage(string message);
    }
}

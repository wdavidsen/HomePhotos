using SCS.HomePhotos.Model;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    public interface IUserData : IDataBase
    {
        Task<User> GetUser(string userName);
    }
}
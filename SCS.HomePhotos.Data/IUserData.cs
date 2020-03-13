using System.Collections.Generic;
using System.Threading.Tasks;
using SCS.HomePhotos.Model;

namespace SCS.HomePhotos.Data
{
    public interface IUserData : IDataBase
    {
        Task<User> GetUser(string userName);
    }
}
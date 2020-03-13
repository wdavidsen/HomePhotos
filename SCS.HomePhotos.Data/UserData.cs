using SCS.HomePhotos.Model;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data
{
    public class UserData : DataBase, IUserData
    {
        public UserData(IStaticConfig staticConfig) : base(staticConfig) { }

        public async Task<User> GetUser(string userName)
        {
            var list = await GetListAsync<User>("WHERE UserName = @UserName", new { UserName = userName });

            return list.FirstOrDefault();
        }
    }
}

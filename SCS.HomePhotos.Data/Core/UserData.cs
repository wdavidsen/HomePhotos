using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Core
{
    /// <summary>
    /// The user repository.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Data.Core.DataBase" />
    /// <seealso cref="SCS.HomePhotos.Data.Contracts.IUserData" />
    public class UserData : DataBase, IUserData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserData"/> class.
        /// </summary>
        /// <param name="staticConfig">The static configuration.</param>
        public UserData(IStaticConfig staticConfig) : base(staticConfig) { }

        /// <summary>
        /// Gets a user by username.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>The user entity.</returns>
        public async Task<User> GetUser(string userName)
        {
            var list = await GetListAsync<User>("WHERE UserName = @UserName", new { UserName = userName });

            return list.FirstOrDefault();
        }
    }
}

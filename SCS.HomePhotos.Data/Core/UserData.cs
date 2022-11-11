using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;

using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Core
{
    /// <summary>
    /// The user repository.
    /// </summary>
    public class UserData : DataBase<User>, IUserData
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
            var list = await GetListAsync("WHERE UserName = @UserName", new { UserName = userName });

            return list.FirstOrDefault();
        }
    }
}

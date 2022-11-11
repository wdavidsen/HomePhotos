using SCS.HomePhotos.Model;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    /// <summary>
    /// The user repository.
    /// </summary>
    public interface IUserData : IDataBase<User>
    {
        /// <summary>
        /// Gets a user by username.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>The user entity.</returns>
        Task<User> GetUser(string userName);
    }
}
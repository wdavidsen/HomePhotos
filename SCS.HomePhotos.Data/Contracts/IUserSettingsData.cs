using SCS.HomePhotos.Model;

using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    /// <summary>
    /// The configuration repository.
    /// </summary>    
    public interface IUserSettingsData : IDataBase<UserSettings>
    {
        /// <summary>
        /// Gets the user's settings.
        /// </summary>
        /// <returns>The user's settings.</returns>
        Task<UserSettings> GetSettings(int userId);

        /// <summary>
        /// Saves the user's settings.
        /// </summary>
        /// <param name="settings">The user's settings.</param>
        /// <returns>A void task.</returns>
        Task SaveSettings(UserSettings settings);
    }
}
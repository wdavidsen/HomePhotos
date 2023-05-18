using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;

using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Core
{
    /// <summary>
    /// The configuration repository.
    /// </summary>
    public class UserSettingsData : DataBase<UserSettings>, IUserSettingsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigData"/> class.
        /// </summary>
        /// <param name="staticConfig">The static configuration.</param>
        public UserSettingsData(IStaticConfig staticConfig) : base(staticConfig) { }

        /// <summary>
        /// Gets the user's settings.
        /// </summary>
        /// <returns>The user's settings.</returns>
        public async Task<UserSettings> GetSettings(int userId)
        {
            var settings = await GetAsync(userId);

            if (settings == null)
            {
                settings = new UserSettings
                {
                    UserId = userId,
                    AutoStartSlideshow = true,
                    DefaultView = string.Empty,
                    SlideshowSpeed = SlideshowSpeed.Normal,
                    ThumbnailSize = ThumbnailSize.Medium,
                    UserScope = UserPhotoScope.Everything
                };
                await InsertAsync(settings);
            }

            return settings;
        }

        /// <summary>
        /// Saves the user's settings.
        /// </summary>
        /// <param name="settings">The user's settings.</param>
        /// <returns>A void task.</returns>
        public async Task SaveSettings(UserSettings settings)
        {
            var currentSettings = await GetSettings(settings.UserId);

            if (currentSettings != null)
            {
                currentSettings.SlideshowSpeed = settings.SlideshowSpeed;
                currentSettings.ThumbnailSize = settings.ThumbnailSize;
                currentSettings.AutoStartSlideshow = settings.AutoStartSlideshow;
                currentSettings.DefaultView = settings.DefaultView;
                currentSettings.UserScope = settings.UserScope;

                await UpdateAsync(currentSettings);
            }
            else
            {
                await InsertAsync(settings);
            }
        }
    }
}

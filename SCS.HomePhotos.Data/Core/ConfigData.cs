using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;

using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Core
{
    /// <summary>
    /// The configuration repository.
    /// </summary>
    public class ConfigData : DataBase<Config>, IConfigData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigData"/> class.
        /// </summary>
        /// <param name="staticConfig">The static configuration.</param>
        public ConfigData(IStaticConfig staticConfig) : base(staticConfig) { }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <returns>The configuration.</returns>
        public async Task<Config> GetConfiguration()
        {
            var configs = await GetListAsync();

            return configs.LastOrDefault();
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The configuration.</returns>
        public async Task<Config> SaveConfiguration(Config config)
        {
            if (config.ConfigId == null)
            {
                var configId = await InsertAsync(config);
                config.ConfigId = configId;
            }
            else
            {
                await UpdateAsync(config);
            }

            return config;
        }
    }
}

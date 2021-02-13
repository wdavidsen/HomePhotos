using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Core
{
    public class ConfigData : DataBase, IConfigData
    {
        public ConfigData(IStaticConfig staticConfig) : base(staticConfig) { }

        public async Task<Config> GetConfiguration()
        {
            var configs = await GetListAsync<Config>();

            return configs.LastOrDefault();
        }

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

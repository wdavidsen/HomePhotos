using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Data;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Service.Contracts;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Core
{
    public class ConfigService : IConfigService
    {
        private readonly ILogger<ConfigService> _logger;

        public ConfigService(ILogger<ConfigService> logger)
        {
            _logger = logger;
        }

        private readonly IConfigData _configData;

        public ConfigService(IConfigData config, IDynamicConfig dynamicConfig, IStaticConfig staticConfig)
        {
            _configData = config;
            DynamicConfig = dynamicConfig;
            StaticConfig = staticConfig;

            DynamicConfig.PropertyChanged += _dynamicConfig_PropertyChanged;
        }

        public IDynamicConfig DynamicConfig { get; }

        public IStaticConfig StaticConfig { get; }

        public async Task SetDynamicConfig()
        {
            var dbConfig = await _configData.GetConfiguration();

            if (dbConfig != null)
            {
                dbConfig.ToDynamicConfig(DynamicConfig);
            }
            else
            {
                dbConfig = new Model.Config();
                dbConfig.FromDynamicConfig(DynamicConfig.GetDefault());
                dbConfig.ToDynamicConfig(DynamicConfig);
                await _configData.SaveConfiguration(dbConfig);
            }

            DynamicConfig.TrackChanges = true;
        }

        public void SaveDynamicConfig()
        {
            var configToSave = new Model.Config();
            configToSave.FromDynamicConfig(DynamicConfig);
            _configData.SaveConfiguration(configToSave);
        }

        private void _dynamicConfig_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveDynamicConfig();
        }
    }
}

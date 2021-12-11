using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Data;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Service.Contracts;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Core
{
    /// <summary>
    /// Configuration that may be changed at runtime.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Service.Contracts.IConfigService" />
    public class ConfigService : IConfigService
    {
        private readonly ILogger<ConfigService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ConfigService(ILogger<ConfigService> logger)
        {
            _logger = logger;
        }

        private readonly IConfigData _configData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigService"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="dynamicConfig">The dynamic configuration.</param>
        /// <param name="staticConfig">The static configuration.</param>
        public ConfigService(IConfigData config, IDynamicConfig dynamicConfig, IStaticConfig staticConfig)
        {
            _configData = config;
            DynamicConfig = dynamicConfig;
            StaticConfig = staticConfig;

            DynamicConfig.PropertyChanged += _dynamicConfig_PropertyChanged;
        }

        /// <summary>
        /// Gets the dynamic configuration.
        /// </summary>
        /// <value>
        /// The dynamic configuration.
        /// </value>
        public IDynamicConfig DynamicConfig { get; }

        /// <summary>
        /// Gets the static configuration.
        /// </summary>
        /// <value>
        /// The static configuration.
        /// </value>
        public IStaticConfig StaticConfig { get; }

        /// <summary>
        /// Sets the dynamic configuration.
        /// </summary>
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

        /// <summary>
        /// Saves the dynamic configuration.
        /// </summary>
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

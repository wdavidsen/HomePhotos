using AutoFixture;
using Moq;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Core;
using System.Linq;
using Xunit;

namespace SCS.HomePhotos.Service.Test
{
    public class ConfigServiceTests
    {
        private readonly Fixture _fixture;

        private readonly IConfigService _configService;
        private readonly Mock<IConfigData> _configData;
        private readonly Mock<IDynamicConfig> _dynamicConfig;
        private readonly Mock<IStaticConfig> _staticConfig;

        public ConfigServiceTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _configData = new Mock<IConfigData>();
            _dynamicConfig = new Mock<IDynamicConfig>();
            _staticConfig = new Mock<IStaticConfig>();

            _dynamicConfig.Setup(m => m.GetDefault()).Returns(new DynamicConfig());

            _configService = new ConfigService(_configData.Object, _dynamicConfig.Object, _staticConfig.Object);
        }

        [Fact]
        public void SetDynamicConfigInsert()
        {
            _configData.Setup(m => m.GetConfiguration())
                .ReturnsAsync(null as Config);

            _configData.Setup(m => m.SaveConfiguration(It.IsAny<Config>()));

            _configService.SetDynamicConfig();

            _configData.Verify(m => m.GetConfiguration(), Times.Once);
            _configData.Verify(m => m.SaveConfiguration(It.IsAny<Config>()), Times.Once);
        }

        [Fact]
        public void SetDynamicConfigUpdate()
        {
            var config = _fixture.Create<Config>();

            _configData.Setup(m => m.GetConfiguration())
                .ReturnsAsync(config);

            _configData.Setup(m => m.SaveConfiguration(It.IsAny<Config>()));

            _configService.SetDynamicConfig();

            _configData.Verify(m => m.GetConfiguration(), Times.Once);
            _configData.Verify(m => m.SaveConfiguration(It.IsAny<Config>()), Times.Never);
        }

        [Fact]
        public void SaveDynamicConfig()
        {
            _configData.Setup(m => m.SaveConfiguration(It.IsAny<Config>()));

            _configService.SaveDynamicConfig();

            _configData.Verify(m => m.SaveConfiguration(It.IsAny<Config>()), Times.Once);
        }
    }
}

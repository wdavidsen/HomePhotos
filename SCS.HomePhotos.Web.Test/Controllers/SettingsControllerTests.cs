using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SCS.HomePhotos.Web.Controllers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Controllers
{
    public class SettingsControllerTests : ControllerTestBase
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly SettingsController _settingsController;
        private readonly Mock<ILogger<SettingsController>> _logger;
        private readonly Mock<IDynamicConfig> _dynamicConfig;

        public SettingsControllerTests()
        {
            _logger = new Mock<ILogger<SettingsController>>();
            _dynamicConfig = new Mock<IDynamicConfig>();

            _settingsController = new SettingsController(_logger.Object, _dynamicConfig.Object, null);
        }

        [Fact]
        public void Get()
        {
            var indexTime = DateTime.Today.AddDays(1);

            _dynamicConfig.SetupGet(p => p.NextIndexTime).Returns(indexTime);

            var response = _settingsController.Get();

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<Dto.Settings>(value);

            var dto = value as Dto.Settings;

            Assert.Equal(indexTime, dto.NextIndexTime);
        }

        [Fact]
        public async Task Put()
        {
            var settings = _fixture.Create<Dto.Settings>();

            var response = await _settingsController.Put(settings);

            Assert.IsType<OkResult>(response);
        }

        protected override void Dispose(bool disposing)
        {
            _settingsController.Dispose();
        }
    }
}

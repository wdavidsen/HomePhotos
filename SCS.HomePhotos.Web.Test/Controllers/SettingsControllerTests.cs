using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Controllers
{
    public class SettingsControllerTests
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
        public void Put()
        {
            var settings = _fixture.Create<Dto.Settings>();

            var response = _settingsController.Put(settings);

            Assert.IsType<OkResult>(response);
        }
    }
}

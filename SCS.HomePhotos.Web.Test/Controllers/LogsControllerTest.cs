using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Web.Controllers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Controllers
{
    public class LogsControllerTest : ControllerTestBase
    {
        private readonly Fixture _fixture;

        private readonly LogsController _logsController;
        private readonly Mock<IAdminLogService> _adminLogService;
        private readonly Mock<ILogger<LogsController>> _logger;

        public LogsControllerTest()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _logger = new Mock<ILogger<LogsController>>();
            _adminLogService = new Mock<IAdminLogService>();

            _logsController = new LogsController(_logger.Object, _adminLogService.Object);
        }

        [Fact]
        public async Task GetLatestLogsTest()
        {
            var pageInfo = new PageInfo();
            var entries = _fixture.CreateMany<LogEntry>(10);
            var data = new DataList<LogEntry>(entries, pageInfo);

            _adminLogService.Setup(m => m.GetLatestLogEntries(It.IsAny<PageInfo>(), null, null))
                .ReturnsAsync(data);

            var response = await _logsController.GetLatestLogs();

            _adminLogService.Verify(m => m.GetLatestLogEntries(It.IsAny<PageInfo>(), null, null),
                Times.Once);

            Assert.IsType<OkObjectResult>(response);

            var value = ((OkObjectResult)response).Value;

            Assert.IsType<DataList<Dto.LogEntry>>(value);
            Assert.Equal(10, ((DataList<Dto.LogEntry>)value).Data.Count);
        }

        protected override void Dispose(bool disposing)
        {
            _logsController.Dispose();
        }
    }
}

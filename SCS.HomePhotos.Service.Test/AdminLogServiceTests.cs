using AutoFixture;
using Moq;
using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SCS.HomePhotos.Service.Test
{
    public class AdminLogServiceTests
    {
        private readonly Fixture _fixture;

        private readonly IAdminLogService _adminLogService;
        private readonly Mock<ILogData> _logData;
        private readonly Mock<IStaticConfig> _staticConfig;

        public AdminLogServiceTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _logData = new Mock<ILogData>();
            _staticConfig = new Mock<IStaticConfig>();

            _adminLogService = new AdminLogService(_logData.Object, _staticConfig.Object);
        }

        [Fact]
        public void GetLatestLogEntries()
        {
            var pageInfo = new PageInfo();
            var category = LogCategory.General;
            var severity = LogSeverity.Neutral;

            var entries = new List<LogEntry>();
            var dataList = new DataList<LogEntry>(entries, pageInfo);

            _logData.Setup(m => m.GetLogEntries(pageInfo, category, severity, DateTime.MinValue, It.IsAny<DateTime>()))
                .Callback<PageInfo, LogCategory?, LogSeverity?, DateTime, DateTime>((pageInfo, category, serverity, timestampStart, timestampEnd) =>
                {
                    Assert.True(timestampEnd > DateTime.Now.AddMinutes(-1));
                })
                .ReturnsAsync(dataList);

            var results = _adminLogService.GetLatestLogEntries(pageInfo, category, severity);

            _logData.Verify(m => m.GetLogEntries(pageInfo, category, severity, DateTime.MinValue, It.IsAny<DateTime>()),
                Times.Once);
        }

        [Fact]
        public void AddLogEntryUpdate()
        {
            var logEntry = _fixture.Create<LogEntry>();

            _logData.Setup(m => m.GetExistingEntry(logEntry.Message, It.IsAny<TimeSpan>()))
                .ReturnsAsync(logEntry);

            _logData.Setup(m => m.InsertAsync(logEntry));
            _logData.Setup(m => m.UpdateAsync(logEntry));

            _adminLogService.AddLogEntry(logEntry, TimeSpan.FromMinutes(1));

            _logData.Verify(m => m.InsertAsync(logEntry), Times.Never);
            _logData.Verify(m => m.UpdateAsync(logEntry), Times.Once);
        }

        [Fact]
        public void AddLogEntryInsert()
        {
            var logEntry = _fixture.Create<LogEntry>();

            _logData.Setup(m => m.GetExistingEntry(logEntry.Message, It.IsAny<TimeSpan>()))
                .ReturnsAsync(null as LogEntry);

            _logData.Setup(m => m.InsertAsync(logEntry));
            _logData.Setup(m => m.UpdateAsync(logEntry));

            _adminLogService.AddLogEntry(logEntry, TimeSpan.FromMinutes(1));

            _logData.Verify(m => m.InsertAsync(logEntry), Times.Once);
            _logData.Verify(m => m.UpdateAsync(logEntry), Times.Never);
        }

        [Fact]
        public void AddLogEntry()
        {
            var logEntry = _fixture.Create<LogEntry>();

            _logData.Setup(m => m.Insert(logEntry));

            _adminLogService.AddLogEntry(logEntry);

            _logData.Verify(m => m.Insert(logEntry), Times.Once);
        }

        [Fact]
        public void LogHigh()
        {
            var entry = _fixture.Create<LogEntry>();
            var message = entry.Message;
            var category = entry.Category;

            _logData.Setup(m => m.Insert(It.IsAny<LogEntry>()))
                .Callback<LogEntry>((entry) =>
                {
                    Assert.True(entry.Timestamp > DateTime.Now.AddMinutes(-1));
                    Assert.Equal(message, entry.Message);
                    Assert.Equal(category, entry.Category);
                    Assert.Equal(LogSeverity.High, entry.Severity);
                });

            var returnMessage = _adminLogService.LogHigh(message, category);

            _logData.Verify(m => m.Insert(It.IsAny<LogEntry>()), Times.Once);

            Assert.Equal(message, returnMessage);
        }

        [Fact]
        public void LogNeutral()
        {
            var entry = _fixture.Create<LogEntry>();
            var message = entry.Message;
            var category = entry.Category;

            _logData.Setup(m => m.Insert(It.IsAny<LogEntry>()))
                .Callback<LogEntry>((entry) =>
                {
                    Assert.True(entry.Timestamp > DateTime.Now.AddMinutes(-1));
                    Assert.Equal(message, entry.Message);
                    Assert.Equal(category, entry.Category);
                    Assert.Equal(LogSeverity.Neutral, entry.Severity);
                });

            var returnMessage = _adminLogService.LogNeutral(message, category);

            _logData.Verify(m => m.Insert(It.IsAny<LogEntry>()), Times.Once);

            Assert.Equal(message, returnMessage);
        }

        [Fact]
        public void LogElevated()
        {
            var entry = _fixture.Create<LogEntry>();
            var message = entry.Message;
            var category = entry.Category;

            _logData.Setup(m => m.Insert(It.IsAny<LogEntry>()))
                .Callback<LogEntry>((entry) =>
                {
                    Assert.True(entry.Timestamp > DateTime.Now.AddMinutes(-1));
                    Assert.Equal(message, entry.Message);
                    Assert.Equal(category, entry.Category);
                    Assert.Equal(LogSeverity.Elevated, entry.Severity);
                });

            var returnMessage = _adminLogService.LogElevated(message, category);

            _logData.Verify(m => m.Insert(It.IsAny<LogEntry>()), Times.Once);

            Assert.Equal(message, returnMessage);
        }

        [Fact]
        public void LogUrgent()
        {
            var entry = _fixture.Create<LogEntry>();
            var message = entry.Message;
            var category = entry.Category;

            _logData.Setup(m => m.Insert(It.IsAny<LogEntry>()))
                .Callback<LogEntry>((entry) =>
                {
                    Assert.True(entry.Timestamp > DateTime.Now.AddMinutes(-1));
                    Assert.Equal(message, entry.Message);
                    Assert.Equal(category, entry.Category);
                    Assert.Equal(LogSeverity.Urgent, entry.Severity);
                });

            var returnMessage = _adminLogService.LogUrgent(message, category);

            _logData.Verify(m => m.Insert(It.IsAny<LogEntry>()), Times.Once);

            Assert.Equal(message, returnMessage);
        }

        [Fact]
        public void TruncateLog()
        {
            _staticConfig.SetupGet(p => p.LogRetentionDays).Returns(10);

            _logData.Setup(m => m.DeleteLogEntries(It.IsAny<DateTime>()))
                .Callback<DateTime>((cutoffTime) => {
                    Assert.InRange<DateTime>(cutoffTime, DateTime.Now.AddDays(-10).AddMinutes(-1), DateTime.Now.AddDays(-10));
                });            

            _adminLogService.TruncateLog();

            _logData.Verify(m => m.DeleteLogEntries(It.IsAny<DateTime>()),
                Times.Once);
        }
    }
}

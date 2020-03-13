using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service.Workers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Workers
{
    /// <summary>
    /// A background queue service.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.BackgroundService" />
    /// <remarks>Taken from: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.0&amp;tabs=visual-studio </remarks>
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedHostedService"/> class.
        /// </summary>
        /// <param name="taskQueue">The task queue.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILoggerFactory loggerFactory)
        {
            TaskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
        }

        /// <summary>
        /// Gets the task queue.
        /// </summary>
        /// <value>
        /// The task queue.
        /// </value>
        public IBackgroundTaskQueue TaskQueue { get; }

        /// <summary>
        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (false == stoppingToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);
                try
                {
                    await workItem(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    continue;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred executing {nameof(workItem)}.");
                }
            }
        }
    }
}

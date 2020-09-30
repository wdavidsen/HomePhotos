using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Workers
{
    /// <summary>
    /// A background task queue.
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Queues the background work item.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <exception cref="ArgumentNullException">workItem</exception>
        void QueueBackgroundWorkItem(Func<CancellationToken, IQueueEvents, Task> workItem);

        /// <summary>
        /// Dequeues the next task.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The next task in queue.</returns>
        Task<Func<CancellationToken, IQueueEvents, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}

using SCS.HomePhotos.Service.Workers;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Workers
{
    /// <summary>
    /// A background task queue.
    /// </summary>
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Func<CancellationToken, IQueueEvents, Task>> _workItems = new ConcurrentQueue<Func<CancellationToken, IQueueEvents, Task>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        /// <summary>
        /// Queues the background work item.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <exception cref="ArgumentNullException">workItem</exception>
        public void QueueBackgroundWorkItem(Func<CancellationToken, IQueueEvents, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        /// <summary>
        /// Dequeues the next task.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The next task in queue.</returns>
        public async Task<Func<CancellationToken, IQueueEvents, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}

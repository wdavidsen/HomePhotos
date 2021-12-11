using System;

namespace SCS.HomePhotos.Service.Workers
{
    /// <summary>
    /// Queue events.
    /// </summary>
    /// <seealso cref="IQueueEvents" />
    public class QueueEvents : IQueueEvents
    {
        /// <summary>
        /// Gets or sets the item processed action with task completion info.
        /// </summary>
        /// <value>
        /// The item processed <see cref="Action" /> with task completion info.
        /// </value>
        public Action<TaskCompleteInfo> ItemProcessed { get; set; }
    }
}

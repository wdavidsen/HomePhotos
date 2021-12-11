using System;

namespace SCS.HomePhotos.Service.Workers
{
    /// <summary>
    /// Queue events.
    /// </summary>
    public interface IQueueEvents
    {
        /// <summary>
        /// Gets or sets the item processed action with task completion info.
        /// </summary>
        /// <value>
        /// The item processed <see cref="Action"/> with task completion info.
        /// </value>
        Action<TaskCompleteInfo> ItemProcessed { get; set; }
    }
}
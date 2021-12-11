using System;

namespace SCS.HomePhotos.Service.Workers
{
    /// <summary>
    /// Photo index events.
    /// </summary>
    public interface IIndexEvents
    {
        /// <summary>
        /// Gets or sets the index completed action.
        /// </summary>
        /// <value>
        /// The index completed <see cref="Action"/>.
        /// </value>
        Action IndexCompleted { get; set; }

        /// <summary>
        /// Gets or sets the index started action.
        /// </summary>
        /// <value>
        /// The index started <see cref="Action"/>.
        /// </value>
        Action IndexStarted { get; set; }

        /// <summary>
        /// Gets or sets the index failed action.
        /// </summary>
        /// <value>
        /// The index failed <see cref="Action"/>.
        /// </value>
        Action IndexFailed { get; set; }
    }
}
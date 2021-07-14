using System.ComponentModel;

namespace SCS.HomePhotos
{
    /// <summary>
    /// The configuration that can be updated by the application during runtime.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.IConfig" />
    public interface IDynamicConfig : IConfig
    {
        /// <summary>
        /// Occurs when a property changed.
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether to track changes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tracking changes; otherwise, <c>false</c>.
        /// </value>
        bool TrackChanges { get; set; }

        /// <summary>
        /// Gets the default configuration.
        /// </summary>
        /// <returns>The default configuration.</returns>
        IDynamicConfig GetDefault();
    }
}
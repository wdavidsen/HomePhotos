using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Contracts
{
    /// <summary>
    /// Configuration that may be changed at runtime.
    /// </summary>
    public interface IConfigService
    {
        /// <summary>
        /// Gets the dynamic configuration.
        /// </summary>
        /// <value>
        /// The dynamic configuration.
        /// </value>
        IDynamicConfig DynamicConfig { get; }

        /// <summary>
        /// Gets the static configuration.
        /// </summary>
        /// <value>
        /// The static configuration.
        /// </value>
        IStaticConfig StaticConfig { get; }

        /// <summary>
        /// Sets the dynamic configuration.
        /// </summary>
        /// <returns></returns>
        Task SetDynamicConfig();

        /// <summary>
        /// Saves the dynamic configuration.
        /// </summary>
        void SaveDynamicConfig();
    }
}
using System.Threading.Tasks;
using SCS.HomePhotos.Model;

namespace SCS.HomePhotos.Data.Contracts
{
    /// <summary>
    /// The confgiration repository.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Data.Contracts.IDataBase" />
    public interface IConfigData : IDataBase
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <returns>The configuration.</returns>
        Task<Config> GetConfiguration();

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The configuration.</returns>
        Task<Config> SaveConfiguration(Config config);
    }
}
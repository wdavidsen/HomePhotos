using SCS.HomePhotos.Data.Contracts;

namespace SCS.HomePhotos.Data.Core
{
    /// <summary>
    /// The skip image repository.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Data.Core.DataBase" />
    /// <seealso cref="SCS.HomePhotos.Data.Contracts.ISkipImageData" />
    public class SkipImageData : DataBase, ISkipImageData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkipImageData"/> class.
        /// </summary>
        /// <param name="staticConfig">The static configuration.</param>
        public SkipImageData(IStaticConfig staticConfig) : base(staticConfig) { }

    }
}

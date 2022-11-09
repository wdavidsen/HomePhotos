using Dapper;

using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        /// <summary>
        /// Test to whether a skip record exits for a particular file path.
        /// </summary>        
        /// <param name="mobileUpload">if set to <c>true</c> file is a mobile upload.</param>
        /// <param name="originalFolder">The file sub-folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>True if a record for file exists; otherwise, false.</returns>
        public async Task<bool> Exists(bool mobileUpload, string originalFolder, string fileName)
        {
            var list = await GetListAsync<SkipImage>("WHERE MobileUpload = @MobileUpload AND OriginalFolder = @OriginalFolder AND FileName = @FileName",
                new { MobileUpload = mobileUpload, OriginalFolder = originalFolder, FileName = fileName });

            return list.Any();
        }
    }
}

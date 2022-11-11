using SCS.HomePhotos.Model;

using System.Threading.Tasks;

namespace SCS.HomePhotos.Data.Contracts
{
    /// <summary>
    /// The skip image repository.
    /// </summary>
    public interface IFileExclusionData : IDataBase<FileExclusion>
    {
        /// <summary>
        /// Test to whether a skip record exits for a particular file path.
        /// </summary>        
        /// <param name="mobileUpload">if set to <c>true</c> file is a mobile upload.</param>
        /// <param name="originalFolder">The file sub-folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>True if a record for file exists; otherwise, false.</returns>
        Task<bool> Exists(bool mobileUpload, string originalFolder, string fileName);
    }
}
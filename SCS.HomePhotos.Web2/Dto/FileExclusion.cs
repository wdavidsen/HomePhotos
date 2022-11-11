namespace SCS.HomePhotos.Web.Dto
{
    /// <summary>
    /// Tab DTO.
    /// </summary>
    public class FileExclusion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileExclusion"/> class.
        /// </summary>
        public FileExclusion() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExclusion"/> class using the domain model class.
        /// </summary>
        /// <param name="fileExclusion">The fileExclusion.</param>
        /// <param name="config">The dynamic configuration.</param>
        public FileExclusion(Model.FileExclusion fileExclusion, IDynamicConfig config)
        {
            var basePath = fileExclusion.MobileUpload ? config.MobileUploadsFolder : config.IndexPath;
            FileExclusionId = fileExclusion.FileExclusionId;            
            FullPath = FilePath.Combine(basePath, fileExclusion.OriginalFolder, fileExclusion.FileName);
        }

        /// <summary>
        /// Gets or sets the skip image identifier.
        /// </summary>
        /// <value>
        /// The skip image identifier.
        /// </value>        
        public int? FileExclusionId { get; set; }

        /// <summary>
        /// Gets or sets the full path to directory or file.
        /// </summary>
        /// <value>
        /// The full path to directory or file.
        /// </value>
        public string FullPath { get; set; }

        /// <summary>
        /// Converts instance to the domain model.
        /// </summary>
        /// <param name="config">The dynamic configuration.</param>
        /// <returns>The domain equivalent instance.</returns>
        public Model.FileExclusion ToModel(IDynamicConfig config)
        {
            var fileExclusion = new Model.FileExclusion();
            var fullPath = FilePath.EnsureCorrectPathSeparator(FullPath);

            string relativePath;

            if (Path.IsPathFullyQualified(fullPath))
            {
                var regPath = fullPath.StartsWith(config.IndexPath, StringComparison.InvariantCultureIgnoreCase);
                var mobilePath = fullPath.StartsWith(config.MobileUploadsFolder, StringComparison.InvariantCultureIgnoreCase);

                if (regPath)
                {
                    relativePath = fullPath[config.IndexPath.Length..];
                }
                else if (mobilePath)
                {
                    relativePath = fullPath[config.MobileUploadsFolder.Length..];
                }
                else
                {
                    throw new FormatException($"Path must be located under the index/mobile uploads folder. Path attempted: {FullPath}.");
                }
            }
            else
            {
                relativePath = fullPath;
            }

            relativePath = FilePath.FormatRelativePath(relativePath);

            fileExclusion.OriginalFolder = Path.HasExtension(relativePath) ? Path.GetDirectoryName(relativePath) : relativePath;
            fileExclusion.FileName = Path.HasExtension(relativePath) ? Path.GetFileName(relativePath) : string.Empty;

            return fileExclusion;
        }
    }
}

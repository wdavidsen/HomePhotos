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
        public FileExclusion(Model.FileExclusion fileExclusion)
        {
            ExclusionId = fileExclusion.FileExclusionId;
            Folder = fileExclusion.OriginalFolder;
            FileName = fileExclusion.FileName;
        }

        /// <summary>
        /// Gets or sets the skip image identifier.
        /// </summary>
        /// <value>
        /// The skip image identifier.
        /// </value>        
        public int? ExclusionId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether image is a mobile upload.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is a mobile upload; otherwise, <c>false</c>.
        /// </value>       
        public bool MobileUpload { get; set; }

        /// <summary>
        /// Gets or sets the folder.
        /// </summary>
        /// <value>
        /// The folder.
        /// </value>
        public string Folder { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Converts instance to the domain model.
        /// </summary>
        /// <returns>The domain equivalent instance.</returns>
        public Model.FileExclusion ToModel()
        {
            var fileExclusion = new Model.FileExclusion
            {
                FileExclusionId = ExclusionId,
                OriginalFolder = Folder,
                FileName = FileName
            };

            return fileExclusion;
        }
    }
}

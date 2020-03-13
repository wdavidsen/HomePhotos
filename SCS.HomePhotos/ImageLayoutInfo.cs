namespace SCS.HomePhotos
{
    /// <summary>
    /// Info that describes an image.
    /// </summary>
    public struct ImageLayoutInfo
    {
        /// <summary>
        /// The layout image type.
        /// </summary>
        public ImageLayoutType LayoutType;

        /// <summary>
        /// The image height/width ratio.
        /// </summary>
        public decimal Ratio;

        /// <summary>
        /// The image height.
        /// </summary>
        public int Height;

        /// <summary>
        /// The image width.
        /// </summary>
        public int Width;

        /// <summary>
        /// Gets the default image info.
        /// </summary>
        /// <value>
        /// The default.
        /// </value>
        public static ImageLayoutInfo Default
        {
            get
            {
                return new ImageLayoutInfo { LayoutType = ImageLayoutType.Unknown, Ratio = 0, Width = 0, Height = 0 };
            }
        }
    }
}

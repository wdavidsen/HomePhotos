namespace SCS.HomePhotos.Data
{
    /// <summary>
    /// Paged list information.
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageInfo"/> class.
        /// </summary>
        public PageInfo()
        {
            PageNum = 1;
            PageSize = 200;
            SortDescending = false;
            SortBy = null;
            TotalRecords = 0;
        }

        /// <summary>
        /// Gets or sets the sort by property.
        /// </summary>
        /// <value>
        /// The sort by property.
        /// </value>
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sort descending.
        /// </summary>
        /// <value>
        ///   <c>true</c> if sorting descending; otherwise, <c>false</c>.
        /// </value>
        public bool SortDescending { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public int PageNum { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total records.
        /// </summary>
        /// <value>
        /// The total records.
        /// </value>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"TotalRecords: {TotalRecords}; SortBy: {SortBy}; SortDescending: {SortDescending}; PageNum: {PageNum}; PageSize: {PageSize}";
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return TotalRecords ^ PageNum ^ PageSize ^ SortDescending.GetHashCode() ^ SortBy.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var pi = obj as PageInfo;

            if (pi == null)
            {
                return false;
            }
            if (TotalRecords != pi.TotalRecords)
            {
                return false;
            }
            if (PageNum != pi.PageNum)
            {
                return false;
            }
            if (PageSize != pi.PageSize)
            {
                return false;
            }
            if (SortBy != pi.SortBy)
            {
                return false;
            }
            if (SortDescending != pi.SortDescending)
            {
                return false;
            }
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace SCS.HomePhotos.Data
{
    /// <summary>
    /// A paged list of data.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public sealed class DataList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataList{T}"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="pageInfo">The page information.</param>
        /// <exception cref="ArgumentNullException">
        /// data
        /// or
        /// pageInfo
        /// </exception>
        public DataList(IEnumerable<T> data, PageInfo pageInfo)
        {
            ArgumentNullException.ThrowIfNull(data);
            ArgumentNullException.ThrowIfNull(pageInfo);

            Data = data.ToList();
            PageInfo = pageInfo;
        }

        /// <summary>
        /// Gets or sets the page data.
        /// </summary>
        /// <value>
        /// The page data.
        /// </value>
        public List<T> Data { get; set; }

        /// <summary>
        /// Gets or sets the page information.
        /// </summary>
        /// <value>
        /// The page information.
        /// </value>
        public PageInfo PageInfo { get; set; }
    }
}

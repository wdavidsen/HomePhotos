using System;
using System.Collections.Generic;
using System.Linq;

namespace SCS.HomePhotos.Data
{
    public class DataList<T>
    {
        public DataList(IEnumerable<T> data, PageInfo pageInfo)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (pageInfo == null)
            {
                throw new ArgumentNullException(nameof(pageInfo));
            }

            Data = data.ToList();
            PageInfo = pageInfo;
        }

        public List<T> Data { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}

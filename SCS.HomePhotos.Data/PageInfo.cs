using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.HomePhotos.Data
{
    public class PageInfo
    {
        public PageInfo()
        {
            PageNum = 1;
            PageSize = 200;
            SortDescending = false;
            SortBy = null;
            TotalRecords = 0;
        }

        public string SortBy { get; set; }
        public bool SortDescending { get; set; }
        public int PageNum { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public override string ToString()
        {
            return $"TotalRecords: {TotalRecords}; SortBy: {SortBy}; SortDescending: {SortDescending}; PageNum: {PageNum}; PageSize: {PageSize}";
        }

        public override int GetHashCode()
        {
            return TotalRecords ^ PageNum ^ PageSize ^ SortDescending.GetHashCode() ^ SortBy.GetHashCode();
        }

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

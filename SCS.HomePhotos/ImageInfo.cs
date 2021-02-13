using System;
using System.Collections.Generic;

namespace SCS.HomePhotos
{
    public class ImageInfo
    {
        public ImageInfo()
        {
            Tags = new List<string>();
        }

        public DateTime DateTaken { get; set; }

        public List<string> Tags { get; set; }
    }
}

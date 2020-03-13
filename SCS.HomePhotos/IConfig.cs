using System;

namespace SCS.HomePhotos
{
    public interface IConfig
    {
        string CacheFolder { get; set; }
        int? ConfigId { get; set; }
        string IndexPath { get; set; }
        int LargeImageSize { get; set; }
        DateTime? NextIndexTime { get; set; }
        int IndexFrequencyHours { get; set; }
        int ThumbnailSize { get; set; }
        int SmallImageSize { get; set; }
        bool IndexOnStartup { get; set; }
    }
}
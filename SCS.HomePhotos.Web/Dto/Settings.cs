using System;

namespace SCS.HomePhotos.Web.Dto
{
    public class Settings
    {
        public Settings() { }

        public Settings(IDynamicConfig config)
        {
            ConfigId = config.ConfigId;
            CacheFolder = config.CacheFolder;
            IndexPath = config.IndexPath;
            LargeImageSize = config.LargeImageSize;
            SmallImageSize = config.SmallImageSize;
            NextIndexTime = config.NextIndexTime;
            IndexFrequencyHours = config.IndexFrequencyHours;
            ThumbnailSize = config.ThumbnailSize;
        }

        public int? ConfigId { get; set; }
        public string CacheFolder { get; set; }
        public string IndexPath { get; set; }
        public int LargeImageSize { get; set; }
        public int SmallImageSize { get; set; }
        public DateTime? NextIndexTime { get; set; }
        public int IndexFrequencyHours { get; set; }
        public int ThumbnailSize { get; set; }

        public Model.Config ToModel()
        {
            return new Model.Config
            {
                ConfigId = ConfigId,
                CacheFolder = CacheFolder,
                IndexPath = IndexPath,
                LargeImageSize = LargeImageSize,
                SmallImageSize = SmallImageSize,
                NextIndexTime = NextIndexTime,
                IndexFrequencyHours = IndexFrequencyHours,
                ThumbnailSize = ThumbnailSize
            };
        }

        public DynamicConfig ToDynamicConfig()
        {
            return new DynamicConfig
            {
                ConfigId = ConfigId,
                CacheFolder = CacheFolder,
                IndexPath = IndexPath,
                LargeImageSize = LargeImageSize,
                SmallImageSize = SmallImageSize,
                NextIndexTime = NextIndexTime,
                IndexFrequencyHours = IndexFrequencyHours,
                ThumbnailSize = ThumbnailSize
            };
        }
    }
}

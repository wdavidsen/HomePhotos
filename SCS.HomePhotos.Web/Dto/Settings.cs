using System;
using System.ComponentModel.DataAnnotations;

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

        [Required]
        public string CacheFolder { get; set; }

        [Required]
        public string IndexPath { get; set; }

        [Required]
        public int LargeImageSize { get; set; }

        [Required]
        public int SmallImageSize { get; set; }

        [Required]
        public DateTime? NextIndexTime { get; set; }

        [Required]
        public int IndexFrequencyHours { get; set; }

        [Required]
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

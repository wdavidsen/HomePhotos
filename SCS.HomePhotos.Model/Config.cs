using Dapper;
using System;

namespace SCS.HomePhotos.Model
{
    [Table("Config")]
    public class Config : IConfig
    {
        [Key]
        public int? ConfigId { get; set; }
        public string IndexPath { get; set; }
        public string CacheFolder { get; set; }
        public int ThumbnailSize { get; set; }
        public int SmallImageSize { get; set; }
        public int LargeImageSize { get; set; }
        public int IndexFrequencyHours { get; set; }
        public DateTime? NextIndexTime { get; set; }

        public bool IndexOnStartup { get; set; }

        public void ToDynamicConfig(IDynamicConfig dynamicConfig)
        {
            dynamicConfig.ConfigId = ConfigId;
            dynamicConfig.CacheFolder = CacheFolder;
            dynamicConfig.IndexPath = IndexPath;
            dynamicConfig.ThumbnailSize = ThumbnailSize;
            dynamicConfig.SmallImageSize = SmallImageSize;
            dynamicConfig.LargeImageSize = LargeImageSize;
            dynamicConfig.NextIndexTime = NextIndexTime;
            dynamicConfig.IndexFrequencyHours = IndexFrequencyHours;
            dynamicConfig.IndexOnStartup = IndexOnStartup;
        }

        public void FromDynamicConfig(IDynamicConfig dynamicConfig)
        {
            ConfigId = dynamicConfig.ConfigId;
            CacheFolder = dynamicConfig.CacheFolder;
            IndexPath = dynamicConfig.IndexPath;
            ThumbnailSize = dynamicConfig.ThumbnailSize;
            SmallImageSize = dynamicConfig.SmallImageSize;
            LargeImageSize = dynamicConfig.LargeImageSize;
            NextIndexTime = dynamicConfig.NextIndexTime;
            IndexFrequencyHours = dynamicConfig.IndexFrequencyHours;
            IndexOnStartup = dynamicConfig.IndexOnStartup;
        }
    }
}

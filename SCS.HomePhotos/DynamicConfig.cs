using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SCS.HomePhotos
{
    public class DynamicConfig : IDynamicConfig, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int? _configId;
        private string _indexPath;
        private string _cacheFolder;
        private int _thumbnailSize;
        private int _smallImageSize;
        private int _largeImageSize;
        private DateTime? _nextIndexTime;
        private int _indexFrequencyHours;

        public bool TrackChanges { get; set; }
        public int? ConfigId
        {
            get
            {
                return _configId;
            }

            set
            {
                if (value != _configId)
                {
                    _configId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string IndexPath
        {
            get
            {
                return _indexPath;
            }

            set
            {
                if (value != _indexPath)
                {
                    _indexPath = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string CacheFolder
        {
            get
            {
                return _cacheFolder;
            }

            set
            {
                if (value != _cacheFolder)
                {
                    _cacheFolder = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int ThumbnailSize
        {
            get
            {
                return _thumbnailSize;
            }

            set
            {
                if (value != _thumbnailSize)
                {
                    _thumbnailSize = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int SmallImageSize
        {
            get
            {
                return _smallImageSize;
            }

            set
            {
                if (value != _smallImageSize)
                {
                    _smallImageSize = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int LargeImageSize
        {
            get
            {
                return _largeImageSize;
            }

            set
            {
                if (value != _largeImageSize)
                {
                    _largeImageSize = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public DateTime? NextIndexTime
        {
            get
            {
                return _nextIndexTime;
            }

            set
            {
                if (value == null && _nextIndexTime != null || value != null && _nextIndexTime == null || value.ToString("g") != _nextIndexTime.ToString("g"))
                {
                    _nextIndexTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IndexOnStartup { get; set; }

        public int IndexFrequencyHours
        {
            get
            {
                return _indexFrequencyHours;
            }

            set
            {
                if (value != _indexFrequencyHours)
                {
                    _indexFrequencyHours = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public IDynamicConfig GetDefault()
        {
            return new DynamicConfig
            {
                ConfigId = null,
                IndexPath = @"C:\HomePhotos\Photos",
                CacheFolder = @"C:\HomePhotos\Cache", /*AppDomain.CurrentDomain.BaseDirectory  + "cache", */
                ThumbnailSize = 256,
                SmallImageSize = 800,
                LargeImageSize = 1920,
                NextIndexTime = null,
                IndexFrequencyHours = 24
            };
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (TrackChanges)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

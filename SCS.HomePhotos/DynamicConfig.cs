using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SCS.HomePhotos
{
    /// <summary>
    /// The configuration that can be updated by the application during runtime.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.IDynamicConfig" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class DynamicConfig : IDynamicConfig, INotifyPropertyChanged
    {
        #region Fields        
        /// <summary>
        /// Occurs when a property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private int? _configId;
        private string _indexPath;
        private string _cacheFolder;
        private string _mobileUploadsFolder;
        private int _thumbnailSize;
        private int _smallImageSize;
        private int _largeImageSize;
        private DateTime? _nextIndexTime;
        private int _indexFrequencyHours;
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether to track changes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tracking changes; otherwise, <c>false</c>.
        /// </value>
        public bool TrackChanges { get; set; }

        /// <summary>
        /// Gets or sets the configuration identifier.
        /// </summary>
        /// <value>
        /// The configuration identifier.
        /// </value>
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

        /// <summary>
        /// Gets or sets the index path.
        /// </summary>
        /// <value>
        /// The index path.
        /// </value>
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

        /// <summary>
        /// Gets or sets the cache folder.
        /// </summary>
        /// <value>
        /// The cache folder.
        /// </value>
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

        /// <summary>
        /// Gets or sets the mobile uploads folder.
        /// </summary>
        /// <value>
        /// The mobile uploads folder.
        /// </value>
        public string MobileUploadsFolder
        {
            get
            {
                return _mobileUploadsFolder;
            }

            set
            {
                if (value != _mobileUploadsFolder)
                {
                    _mobileUploadsFolder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the thumbnail.
        /// </summary>
        /// <value>
        /// The size of the thumbnail.
        /// </value>
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

        /// <summary>
        /// Gets or sets the size of the small image.
        /// </summary>
        /// <value>
        /// The size of the small image.
        /// </value>
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

        /// <summary>
        /// Gets or sets the size of the large image.
        /// </summary>
        /// <value>
        /// The size of the large image.
        /// </value>
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

        /// <summary>
        /// Gets or sets the next index time.
        /// </summary>
        /// <value>
        /// The next index time.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether to index photos on startup.
        /// </summary>
        /// <value>
        ///   <c>true</c> if indexing photos on startup; otherwise, <c>false</c>.
        /// </value>
        public bool IndexOnStartup { get; set; }

        /// <summary>
        /// Gets or sets the index frequency hours.
        /// </summary>
        /// <value>
        /// The index frequency hours.
        /// </value>
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

        /// <summary>
        /// Gets or sets the photo delete action.
        /// </summary>
        /// <value>
        /// The photo delete action.
        /// </value>
        public DeleteAction PhotoDeleteAction { get; set; }

        /// <summary>
        /// Gets or sets the mobile photo delete action.
        /// </summary>
        /// <value>
        /// The mobile photo delete action.
        /// </value>
        public DeleteAction MobilePhotoDeleteAction { get; set; }

        /// <summary>
        /// Gets or sets the color of the tag.
        /// </summary>
        /// <value>
        /// The color of the tag.
        /// </value>
        public string TagColor { get; set; }

        /// <summary>
        /// Gets the default configuration.
        /// </summary>
        /// <returns>The default configuration.</returns>
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
                IndexFrequencyHours = 24,
                PhotoDeleteAction = DeleteAction.DeleteRecord,
                MobilePhotoDeleteAction = DeleteAction.DeleteRecordAndFile,
                TagColor = Constants.DefaultTagColor
            };
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (TrackChanges)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

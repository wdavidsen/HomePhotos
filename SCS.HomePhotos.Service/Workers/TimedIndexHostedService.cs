using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Workers;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using static System.Net.WebRequestMethods;

namespace SCS.HomePhotos.Workers
{
    /// <summary>
    /// A timed background task service.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
    /// <seealso cref="System.IDisposable" />
    public class TimedIndexHostedService : IHostedService, IDisposable
    {
        private static Collection<string> _allowedExtensions = new Collection<string> { "JPG", "JPEG", "PNG" };
        private int executionCount = 0;
        private bool _indexingNow = false;
        private CancellationToken _indexCanellationToken;
        private CancellationTokenSource _internalCanellationTokenSource;
        private CancellationToken _internalCanellationToken;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TimedIndexHostedService> _logger;
        private readonly IConfigService _configService;
        private readonly IAdminLogService _adminlogger;
        private readonly IIndexEvents _indexEvents;
        private readonly IImageMetadataService _metadataService;

        private Timer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedIndexHostedService"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configService">The configuration service.</param>
        /// <param name="dblogger">The dblogger.</param>
        /// <param name="indexEvents">The index events.</param>
        /// <param name="metadataService">The metadata service.</param>
        public TimedIndexHostedService(IServiceProvider services, ILogger<TimedIndexHostedService> logger, IConfigService configService,
            IAdminLogService dblogger, IIndexEvents indexEvents, IImageMetadataService metadataService)
        {
            Name = "Image Indexer";

            _serviceProvider = services;
            _logger = logger;
            _configService = configService;
            _adminlogger = dblogger;
            _indexEvents = indexEvents;
            _metadataService = metadataService;

            configService.DynamicConfig.PropertyChanged += _config_PropertyChanged;
        }

        /// <summary>
        /// Starts the background service.
        /// </summary>
        /// <param name="stoppingToken">The stopping token.</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _indexCanellationToken = stoppingToken;

            if (SetupTimer())
            {
                StartTimer();
            }
            return Task.CompletedTask;
        }


        /// <summary>
        /// Stops the background processing.
        /// </summary>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            StopTimer();

            _adminlogger.LogNeutral($"Photo indexing stopped.", LogCategory.Index);
            _logger.LogInformation("Photo indexing stopped.");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the name of background service.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public TimeSpan StartTime { get; private set; }

        /// <summary>
        /// Gets the run frequency.
        /// </summary>
        /// <value>
        /// The run frequency.
        /// </value>
        public TimeSpan RunFrequency { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _timer?.Dispose();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        protected void StopTimer()
        {
            _timer?.Change(Timeout.Infinite, 0);
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        protected void StartTimer()
        {
            if (_configService.DynamicConfig.NextIndexTime != null)
            {
                var start = (DateTime.UtcNow + StartTime).ToString("g");

                _timer = new Timer(DoWork, null, StartTime, RunFrequency);

                var msg = $"Next photo index time set for {start} (UTC); reoccurance every {_configService.DynamicConfig.IndexFrequencyHours} hours.";
                _adminlogger.LogNeutral(msg, LogCategory.Index);

                _logger.LogInformation("Next photo index time set for {start} (UTC); reoccurance every {_configService.DynamicConfig.IndexFrequencyHours} hours.",
                    start, _configService.DynamicConfig.IndexFrequencyHours);
            }
        }

        /// <summary>
        /// Setups the timer.
        /// </summary>
        /// <returns></returns>
        protected bool SetupTimer()
        {
            if (_configService.DynamicConfig.NextIndexTime != null)
            {
                _internalCanellationTokenSource = new CancellationTokenSource();
                _internalCanellationToken = _internalCanellationTokenSource.Token;

                StartTime = GetNextStartTime();
                RunFrequency = TimeSpan.FromHours(_configService.DynamicConfig.IndexFrequencyHours);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Does the work.
        /// </summary>
        /// <param name="state">The state.</param>
        protected void DoWork(object state)
        {
            if (_indexingNow)
            {
                return;
            }

            executionCount++;

            _adminlogger.LogNeutral("Photo index started.", LogCategory.Index);
            _logger.LogInformation("Photo index started.");

            try
            {
                // to resume index after crash of power outage
                _configService.DynamicConfig.IndexOnStartup = true;

                var indexPaths = new string[] {
                    _configService.DynamicConfig.IndexPath,
                    _configService.DynamicConfig.MobileUploadsFolder
                };

                _indexEvents.IndexStarted?.Invoke();

                ProcessDirectories(indexPaths, _internalCanellationToken, _indexCanellationToken);
                _logger.LogInformation("Completed photo image index");

                _indexEvents.IndexCompleted?.Invoke();

                var nextStart = _configService.DynamicConfig.NextIndexTime + GetNextStartTime();

                var msg = $"Photo index completed. Next photo index time: {nextStart.ToString("g")} (UTC).";
                _adminlogger.LogNeutral(msg, LogCategory.Index);

                _configService.DynamicConfig.PropertyChanged -= _config_PropertyChanged;
                _configService.DynamicConfig.NextIndexTime = nextStart;
                _configService.DynamicConfig.IndexOnStartup = false;
                _configService.DynamicConfig.PropertyChanged += _config_PropertyChanged;
            }
            catch (AggregateException ex)
            {
                _indexEvents.IndexStarted?.Invoke();

                _adminlogger.LogHigh($"Failed to index photos at path: {_configService.DynamicConfig.IndexPath}", LogCategory.Index);
                _logger.LogError("Failed to index photos at path: {_configService.DynamicConfig.IndexPath}.", _configService.DynamicConfig.IndexPath);

                foreach (var innerEx in ex.InnerExceptions)
                {
                    _logger.LogError(innerEx, "Photo index task failed.");
                }

                StopTimer();
            }
            catch (Exception ex)
            {
                _adminlogger.LogHigh($"Failed to index photos at path: {_configService.DynamicConfig.IndexPath}", LogCategory.Index);
                _logger.LogError(ex, "Failed to index photos at path: {_configService.DynamicConfig.IndexPath}.", _configService.DynamicConfig.IndexPath);
                StopTimer();
            }
            finally
            {
                _indexingNow = false;
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the _config control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void _config_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DynamicConfig.IndexFrequencyHours) || e.PropertyName == nameof(DynamicConfig.NextIndexTime))
            {
                _adminlogger.LogNeutral("Index configuration has changed.", LogCategory.Index);
                _logger.LogInformation("Index configuration has changed.");

                if (_indexingNow && _internalCanellationTokenSource != null)
                {
                    _adminlogger.LogNeutral("Indexing canceled.", LogCategory.Index);
                    _logger.LogInformation("Indexing canceled.");

                    _internalCanellationTokenSource.Cancel();
                    Thread.Sleep(5000);
                }

                if (SetupTimer())
                {
                    StartTimer();
                }
            }
        }

        /// <summary>
        /// Gets the next start time.
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetNextStartTime()
        {
            TimeSpan timespan = _configService.DynamicConfig.NextIndexTime.Value - DateTime.UtcNow;

            while (timespan.TotalMilliseconds < 0)
            {
                timespan = timespan.Add(TimeSpan.FromHours(_configService.DynamicConfig.IndexFrequencyHours));
            }

            return timespan;
        }

        /// <summary>
        /// Processes the directories.
        /// </summary>
        /// <param name="directoryPaths">The directory paths.</param>
        /// <param name="internalCancellationToken">The internal cancellation token.</param>
        /// <param name="externalCancellationToken">The external cancellation token.</param>
        private void ProcessDirectories(string[] directoryPaths, CancellationToken internalCancellationToken, CancellationToken externalCancellationToken)
        {
            foreach (var dir in directoryPaths)
            {
                ProcessDirectory(dir, internalCancellationToken, externalCancellationToken);
            }
        }

        /// <summary>
        /// Processes the directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="internalCancellationToken">The internal cancellation token.</param>
        /// <param name="externalCancellationToken">The external cancellation token.</param>
        private void ProcessDirectory(string directoryPath, CancellationToken internalCancellationToken, CancellationToken externalCancellationToken)
        {
            _logger.LogInformation("Processing directory {DirectoryPath}.", directoryPath);

            using (var scope = _serviceProvider.CreateScope())
            {
                var photoService = scope.ServiceProvider.GetRequiredService<IPhotoService>();
                var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
                var fileSystemService = scope.ServiceProvider.GetRequiredService<IFileSystemService>();

                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = _configService.StaticConfig.ImageIndexParallelism };
                var failCount = 0;

                Parallel.ForEach(Directory.GetFiles(directoryPath), parallelOptions, (imageFilePath) =>
                {
                    if (_allowedExtensions.Contains(Path.GetExtension(imageFilePath).TrimStart('.').ToUpper()))
                    {
                        try
                        {
                            if (fileSystemService.GetFileSize(imageFilePath) <= _configService.StaticConfig.MaxImageFileSizeBytes)
                            {
                                _logger.LogInformation("Processing image file {ImageFilePath}.", imageFilePath);

                                externalCancellationToken.ThrowIfCancellationRequested();
                                internalCancellationToken.ThrowIfCancellationRequested();

                                var checksum = fileSystemService.GetChecksum(imageFilePath);

                                _logger.LogInformation("Determined file checksum: {Checksum}.", checksum);

                                var existingPhoto = photoService.GetPhotoByChecksum(checksum).Result;

                                if (existingPhoto == null || existingPhoto.ReprocessCache || !CacheFileExists(existingPhoto))
                                {
                                    var exifData = _metadataService.GetExifData(imageFilePath);

                                    var cacheFilePath = imageService.CreateCachePath(checksum, Path.GetExtension(imageFilePath));
                                    var fullImagePath = imageService.CreateFullImage(imageFilePath, cacheFilePath);

                                    imageService.OrientImage(fullImagePath, exifData);
                                    var imageLayoutInfo = imageService.GetImageLayoutInfo(fullImagePath);

                                    var smallImagePath = imageService.CreateSmallImage(fullImagePath, cacheFilePath);
                                    imageService.CreateThumbnail(smallImagePath, cacheFilePath);
                                    imageService.SavePhotoAndTags(existingPhoto, imageFilePath, cacheFilePath, checksum, imageLayoutInfo, exifData);
                                }
                                else
                                {
                                    _logger.LogInformation("Skipping image file since its checksum already exists in the database.");
                                }
                            }
                            else
                            {
                                _logger.LogInformation("Skipping image file since it is greater than {MaxImageFileSizeBytes} bytes.", _configService.StaticConfig.MaxImageFileSizeBytes);
                            }
                        }
                        catch (OutOfMemoryException)
                        {
                            throw;
                        }
                        catch (TaskCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            failCount++;
                            _logger.LogError(ex, "Photo image index failed for: {imageFilePath}", imageFilePath);

                            if (ex is IOException)
                            {
                                if (ex.Message.Contains("MEMORY", StringComparison.InvariantCultureIgnoreCase) || ex.Message.Contains("NO SPACE", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    throw;
                                }
                            }
                            if (failCount > _configService.StaticConfig.MaxAllowedIndexDirectoryFailures)
                            {
                                throw;
                            }
                        }
                    }
                });

                foreach (var dirPath in Directory.GetDirectories(directoryPath))
                {
                    ProcessDirectory(dirPath, internalCancellationToken, externalCancellationToken);
                }
            }
        }

        /// <summary>
        /// Determinds whether the cache file exists.
        /// </summary>
        /// <param name="existingPhoto">The existing photo.</param>
        /// <returns>A flag whether the cache file exists.</returns>
        private bool CacheFileExists(Photo existingPhoto)
        {
            return System.IO.File.Exists(Path.Combine(_configService.DynamicConfig.CacheFolder, existingPhoto.CacheFolder, "Thumb", existingPhoto.FileName));
        }
    }
}

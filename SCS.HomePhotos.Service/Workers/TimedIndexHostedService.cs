using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Service;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly IDynamicConfig _dynamicConfig;
        private readonly IStaticConfig _staticConfig;
        private Timer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedIndexHostedService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TimedIndexHostedService(IServiceProvider services, ILogger<TimedIndexHostedService> logger, IDynamicConfig dynamicConfig, IStaticConfig staticConfig)
        {
            Name = "Image Indexer";

            _serviceProvider = services;
            _logger = logger;
            _dynamicConfig = dynamicConfig;
            _staticConfig = staticConfig;

            _dynamicConfig.PropertyChanged += _config_PropertyChanged;
        }

        private void _config_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DynamicConfig.IndexFrequencyHours) || e.PropertyName == nameof(DynamicConfig.NextIndexTime))
            {
                _logger.LogInformation("Index configuration has changed.");

                if (_indexingNow && _internalCanellationTokenSource != null)
                {
                    _logger.LogInformation("Canceling current index now.");

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

        protected void StartTimer()
        {
            if (_dynamicConfig.NextIndexTime != null)
            {
                _timer = new Timer(DoWork, null, StartTime, RunFrequency);

                _logger.LogInformation("Initialized next photo index time. Start time: {StartTime}, Run frequency {RunFrequency}",
                    DateTime.Now + StartTime, RunFrequency);
            }
        }

        protected bool SetupTimer()
        {
            if (_dynamicConfig.NextIndexTime != null)
            {
                _internalCanellationTokenSource = new CancellationTokenSource();
                _internalCanellationToken = _internalCanellationTokenSource.Token;

                TimeSpan timespan = _dynamicConfig.NextIndexTime.Value - DateTime.Now;

                while (timespan.Minutes < 0)
                {
                    timespan = timespan.Add(TimeSpan.FromHours(_dynamicConfig.IndexFrequencyHours));
                }

                StartTime = timespan;
                RunFrequency = TimeSpan.FromHours(_dynamicConfig.IndexFrequencyHours);

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

            _logger.LogInformation("Photo index service is executing. Count: {Count}", executionCount);

            try
            {
                // to resume index after crash of power outage
                _dynamicConfig.IndexOnStartup = true;

                ProcessDirectory(_indexCanellationToken, _internalCanellationToken, _dynamicConfig.IndexPath);
                _logger.LogInformation("Completed photo image index");

                _dynamicConfig.IndexOnStartup = false;
            }
            catch (AggregateException ex)
            {
                _logger.LogError("Failed to index photos at path: {IndexPath}", _dynamicConfig.IndexPath);

                foreach (var innerEx in ex.InnerExceptions)
                {
                    _logger.LogError(innerEx, "Photo index task failed.");
                }
                
                StopTimer();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to index photos at path: {IndexPath}", _dynamicConfig.IndexPath);         
                StopTimer();
            }
            finally
            {
                _indexingNow = false;
            }
        }

        /// <summary>
        /// Stops the background processing.
        /// </summary>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Photo index service is stopping.");

            StopTimer();

            return Task.CompletedTask;
        }

        protected void StopTimer()
        {
            _timer?.Change(Timeout.Infinite, 0);
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
            _timer?.Dispose();
        }

        private void ProcessDirectory(CancellationToken externalCancellationToken, CancellationToken internalCancellationToken, string directoryPath)
        {
            _logger.LogInformation("Processing directory {DirectoryPath}.", directoryPath);

            using (var scope = _serviceProvider.CreateScope())
            {
                var photoService = scope.ServiceProvider.GetRequiredService<IPhotoService>();
                var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
                var fileSystemService = scope.ServiceProvider.GetRequiredService<IFileSystemService>();

                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = _staticConfig.ImageIndexParallelism };
                var failCount = 0;

                Parallel.ForEach(Directory.GetFiles(directoryPath), parallelOptions, (imageFilePath) =>
                {
                    if (_allowedExtensions.Contains(Path.GetExtension(imageFilePath).TrimStart('.').ToUpper()))
                    {
                        try
                        {
                            if (fileSystemService.GetFileSize(imageFilePath) <= _staticConfig.MaxImageFileSizeBytes)
                            {
                                _logger.LogInformation("Processing image file {ImageFilePath}.", imageFilePath);

                                externalCancellationToken.ThrowIfCancellationRequested();
                                internalCancellationToken.ThrowIfCancellationRequested();

                                var checksum = fileSystemService.GetChecksum(imageFilePath);

                                _logger.LogInformation("Determined file checksum: {Checksum}.", checksum);

                                if (photoService.GetPhotoByChecksum(checksum).Result == null)
                                {
                                    var cacheFilePath = imageService.CreateCachePath(checksum, Path.GetExtension(imageFilePath));
                                    var imageLayoutInfo = imageService.GetImageLayoutInfo(imageFilePath);
                                    var fullImagePath = imageService.CreateFullImage(imageFilePath, cacheFilePath);
                                    var smallImagePath = imageService.CreateSmallImage(fullImagePath, cacheFilePath);
                                    imageService.CreateThumbnail(smallImagePath, cacheFilePath);
                                    imageService.SavePhotoAndTags(imageFilePath, cacheFilePath, checksum);
                                }
                                else
                                {
                                    _logger.LogInformation("Skipping image file since its checksum already exists in the database.");
                                }
                            }
                            else
                            {
                                _logger.LogInformation("Skipping image file since it is greater than {MaxImageFileSizeBytes} bytes.", _staticConfig.MaxImageFileSizeBytes);
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
                            _logger.LogError(ex, $"Photo image index failed for: {imageFilePath}");

                            if (ex is IOException)
                            {
                                if (ex.Message.Contains("MEMORY", StringComparison.InvariantCultureIgnoreCase) || ex.Message.Contains("NO SPACE", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    throw;
                                }
                            }
                            if (failCount > _staticConfig.MaxAllowedIndexDirectoryFailures)
                            {
                                throw;
                            }
                        }
                    }
                });

                foreach (var dirPath in Directory.GetDirectories(directoryPath))
                {
                    ProcessDirectory(externalCancellationToken, internalCancellationToken, dirPath);
                }
            }
        }
    }
}

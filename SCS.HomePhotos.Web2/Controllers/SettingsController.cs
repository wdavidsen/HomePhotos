using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Web.Dto;

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>Settings services.</summary>
    [Authorize]
    [Authorize(Policy = "Admins")]
    [Route("api/[controller]")]
    public class SettingsController : HomePhotosController
    {
        private readonly ILogger<SettingsController> _logger;
        private readonly IDynamicConfig _dynamicConfig;
        private readonly IPhotoService _photoService;
        private readonly IAdminLogService _adminLogService;

        /// <summary>Initializes a new instance of the <see cref="SettingsController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="adminLogService">The admin logger.</param>
        /// <param name="dynamicConfig">The dynamic configuration.</param>
        /// <param name="photoService">The photo service.</param>
        public SettingsController(ILogger<SettingsController> logger, IAdminLogService adminLogService, IDynamicConfig dynamicConfig, IPhotoService photoService)
        {
            _logger = logger;
            _dynamicConfig = dynamicConfig;
            _photoService = photoService;
            _adminLogService = adminLogService;
        }

        /// <summary>Gets the settings.</summary>
        /// <returns>The settings.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.Settings))]
        [HttpGet]
        public IActionResult Get()
        {
            var settings = new Dto.Settings(_dynamicConfig);

            return Ok(settings);
        }

        /// <summary>Updates settings.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="reprocessPhotos">if set to <c>true</c> reprocess photos.</param>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Put([FromBody] Dto.Settings settings, bool reprocessPhotos = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (reprocessPhotos && ImageSizeChanged(_dynamicConfig, settings))
            {
                await _photoService.FlagPhotosForReprocessing();
            }

            _dynamicConfig.CacheFolder = settings.CacheFolder;
            _dynamicConfig.MobileUploadsFolder = settings.MobileUploadsFolder;
            _dynamicConfig.IndexPath = settings.IndexPath;
            _dynamicConfig.NextIndexTime = settings.NextIndexTime;
            _dynamicConfig.IndexFrequencyHours = settings.IndexFrequencyHours;
            _dynamicConfig.LargeImageSize = settings.LargeImageSize;
            _dynamicConfig.ThumbnailSize = settings.ThumbnailSize;
            _dynamicConfig.PhotoDeleteAction = (DeleteAction)settings.PhotoDeleteAction;
            _dynamicConfig.MobilePhotoDeleteAction = (DeleteAction)settings.MobilePhotoDeleteAction;
            _dynamicConfig.TagColor = settings.TagColor;

            _adminLogService.LogElevated($"Application settings have been updated.", LogCategory.Security);

            return Ok();
        }

        /// <summary>Initiates photo indexing immediately.</summary>
        /// <param name="reprocessPhotos">if set to <c>true</c> reprocess photos.</param>
        /// <returns>The settings.</returns>
        [HttpPut("indexNow", Name = "UpdateIndex")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.Settings))]
        public async Task<IActionResult> UpdateNow([FromQuery] bool reprocessPhotos = false)
        {
            _adminLogService.LogElevated($"Image index has been manually triggered by {User.Identity.Name}.", LogCategory.Index);

            if (reprocessPhotos)
            {
                await _photoService.FlagPhotosForReprocessing();
            }
            _dynamicConfig.NextIndexTime = DateTime.UtcNow.AddSeconds(5);

            var settings = new Dto.Settings(_dynamicConfig);

            return Ok(settings);
        }

        /// <summary>Clears the application of all photos and tags and deletes the image file cache.</summary>
        [HttpPut("factoryReset", Name = "FactoryReset")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FactoryReset()
        {
            _adminLogService.LogHigh($"Factory reset has been triggered by {User.Identity.Name}.", LogCategory.Security);

            _photoService.SetUserContext(User);
            await _photoService.ResetPhotosAndTags(User.Identity.Name);

            return Ok();
        }

        private static bool ImageSizeChanged(IDynamicConfig dynamicConfig, Settings settings)
        {
            if (dynamicConfig.ThumbnailSize != settings.ThumbnailSize)
            {
                return true;
            }
            if (dynamicConfig.SmallImageSize != settings.SmallImageSize)
            {
                return true;
            }
            if (dynamicConfig.LargeImageSize != settings.LargeImageSize)
            {
                return true;
            }
            return false;
        }
    }
}
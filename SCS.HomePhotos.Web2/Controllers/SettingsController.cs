using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Core;
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

        /// <summary>Initializes a new instance of the <see cref="SettingsController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dynamicConfig">The dynamic configuration.</param>
        /// <param name="photoService">The photo service.</param>
        public SettingsController(ILogger<SettingsController> logger, IDynamicConfig dynamicConfig, IPhotoService photoService)
        {
            _logger = logger;
            _dynamicConfig = dynamicConfig;
            _photoService = photoService;

            _photoService.UserContext = User;
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
            if (reprocessPhotos)
            {
                await _photoService.FlagPhotosForReprocessing();
            }
            _dynamicConfig.NextIndexTime = DateTime.UtcNow.AddSeconds(5);

            var settings = new Dto.Settings(_dynamicConfig);

            return Ok(settings);
        }

        /// <summary>Clears the photo cache.</summary>
        [HttpPut("clearCache", Name = "ClearCache")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearCache()
        {
            await _photoService.DeletePhotoCache(User.Identity.Name);

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
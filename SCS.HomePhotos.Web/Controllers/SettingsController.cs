using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Dto;
using System;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Controllers
{
    [Authorize]
    [Authorize(Policy = "Admins")]
    [Route("api/[controller]")]
    public class SettingsController : HomePhotosController
    {
        private readonly ILogger<SettingsController> _logger;
        private readonly IDynamicConfig _dynamicConfig;
        private readonly IPhotoService _photoService;

        public SettingsController(ILogger<SettingsController> logger, IDynamicConfig dynamicConfig, IPhotoService photoService)
        {
            _logger = logger;
            _dynamicConfig = dynamicConfig;
            _photoService = photoService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var settings = new Dto.Settings(_dynamicConfig);

            return Ok(settings);
        }

        [HttpPut]
        //[ValidateAntiForgeryToken]
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

            return Ok();
        }

        [HttpPut("indexNow", Name = "UpdateIndex")]
        //[ValidateAntiForgeryToken]
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

        [HttpPut("clearCache", Name = "ClearCache")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCache()
        {
            await _photoService.DeletePhotoCache(User.Identity.Name);

            return Ok();
        }

        private bool ImageSizeChanged(IDynamicConfig dynamicConfig, Settings settings)
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
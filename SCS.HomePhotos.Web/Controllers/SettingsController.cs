using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service;
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
        public IActionResult Put([FromBody] Dto.Settings settings, bool reprocessPhotos = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
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
        public async Task<IActionResult> UpdateNow([FromQuery] bool reprocessPhotos = false)
        {
            if (reprocessPhotos)
            {
                await _photoService.FlagPhotosForReprocessing();
            }
            _dynamicConfig.NextIndexTime = DateTime.Now.AddSeconds(5);

            return Ok();
        }

        [HttpPut("clearCache", Name = "ClearCache")]
        public async Task<IActionResult> ClearCache()
        {
            await _photoService.DeletePhotoCache();

            return Ok();
        }
    }
}
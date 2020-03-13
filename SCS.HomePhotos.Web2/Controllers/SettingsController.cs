using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SCS.HomePhotos.Web.Controllers
{
    [Authorize]
    //[Authorize(Policy = "AdminsOnly")]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ILogger<SettingsController> _logger;
        private readonly IDynamicConfig _dynamicConfig;

        public SettingsController(ILogger<SettingsController> logger, IDynamicConfig dynamicConfig)
        {
            _logger = logger;
            _dynamicConfig = dynamicConfig;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var settings = new Dto.Settings(_dynamicConfig);

            return Ok(settings);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Dto.Settings settings)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _dynamicConfig.CacheFolder = settings.CacheFolder;
            _dynamicConfig.IndexPath = settings.IndexPath;
            _dynamicConfig.NextIndexTime = settings.NextIndexTime;
            _dynamicConfig.IndexFrequencyHours = settings.IndexFrequencyHours;
            _dynamicConfig.LargeImageSize = settings.LargeImageSize;
            _dynamicConfig.ThumbnailSize = settings.ThumbnailSize;

            return Ok();
        }
    }
}
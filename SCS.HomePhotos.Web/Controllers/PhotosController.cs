using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PhotosController : ControllerBase
    {
        private readonly ILogger<PhotosController> _logger;
        private readonly IPhotoService _photoSevice;

        public PhotosController(ILogger<PhotosController> logger, IPhotoService photoSevice)
        {
            _logger = logger;
            _photoSevice = photoSevice;
        }

        [HttpGet("latest", Name = "GetLatest")]
        public async Task<IActionResult> GetLatest([FromQuery] int pageNum = 1, [FromQuery] int pageSize = 200)
        {
            var photos = await _photoSevice.GetLatestPhotos(pageNum, pageSize);

            var dtos = new List<Dto.Photo>();

            foreach (var photo in photos)
            {
                dtos.Add(new Dto.Photo(photo));
            }

            return Ok(dtos);
        }

        [HttpGet("byTag", Name = "GetByTag")]
        public async Task<IActionResult> GetByTag([FromQuery] string tag, [FromQuery] int pageNum = 1, [FromQuery] int pageSize = 200)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return BadRequest();
            }

            var photos = await _photoSevice.GetPhotosByTag(new string[] { tag }, pageNum, pageSize);

            var dtos = new List<Dto.Photo>();

            foreach (var photo in photos)
            {
                dtos.Add(new Dto.Photo(photo));
            }

            return Ok(dtos);
        }

        [HttpGet("search", Name = "SearchPhotos")]
        public async Task<IActionResult> SearchPhotos([FromQuery] string keywords, [FromQuery] int pageNum = 1, [FromQuery] int pageSize = 200)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                return BadRequest();
            }

            var photos = await _photoSevice.GetPhotosByKeywords(keywords, pageNum, pageSize);

            var dtos = new List<Dto.Photo>();

            foreach (var photo in photos)
            {
                dtos.Add(new Dto.Photo(photo));
            }

            return Ok(dtos);
        }
    }
}

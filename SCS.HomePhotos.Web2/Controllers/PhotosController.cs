using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Web.Models;

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>Photo services.</summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PhotosController : HomePhotosController
    {
        private readonly ILogger<PhotosController> _logger;        
        private readonly IPhotoService _photoService;
        private readonly IStaticConfig _staticConfig;

        /// <summary>Initializes a new instance of the <see cref="PhotosController" /> class.</summary>
        /// <param name="logger">The logger.</param>        
        /// <param name="photoSevice">The photo service.</param>
        /// <param name="staticConfig">The static configuration.</param>
        public PhotosController(ILogger<PhotosController> logger, IPhotoService photoSevice, IStaticConfig staticConfig)
        {
            _logger = logger;            
            _photoService = photoSevice;
            _staticConfig = staticConfig;
        }

        /// <summary>Gets the latest photos.</summary>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of photos.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Dto.Photo>))]
        [HttpGet("latest", Name = "GetLatestPhotos")]
        public async Task<IActionResult> GetLatestPhotos([FromQuery] int pageNum = 1, [FromQuery] int pageSize = 400)
        {
            _photoService.SetUserContext(User);
            var photos = await _photoService.GetLatestPhotos(pageNum, pageSize);

            var dtos = new List<Dto.Photo>();

            foreach (var photo in photos)
            {
                photo.FileName = GetEncryptedFileName(photo.FileName);
                dtos.Add(new Dto.Photo(photo));
            }

            return Ok(dtos);
        }

        /// <summary>Gets photos by tag.</summary>
        /// <param name="tag">The tag.</param>
        /// <param name="owner">The tag owner username.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of photos.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Dto.Photo>))]
        [HttpGet("byTag", Name = "GetPhotosByTag")]
        public async Task<IActionResult> GetPhotosByTag([FromQuery] string tag, [FromQuery] string owner, [FromQuery] int pageNum = 1, [FromQuery] int pageSize = 400)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return BadRequest();
            }

            IEnumerable<Photo> photos;

            try
            {
                _photoService.SetUserContext(User);
                photos = await _photoService.GetPhotosByTag(tag, owner, pageNum, pageSize);
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(new ProblemModel { Message = ex.Message });
            }

            var dtos = new List<Dto.Photo>();

            foreach (var photo in photos)
            {
                photo.FileName = GetEncryptedFileName(photo.FileName);
                dtos.Add(new Dto.Photo(photo));
            }

            return Ok(dtos);
        }

        /// <summary>Searches photos.</summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="fromDate">The optional date to start search.</param>
        /// <param name="toDate">The optional date to end search.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of photos.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Dto.Photo>))]
        [HttpGet("search", Name = "SearchPhotos")]
        public async Task<IActionResult> SearchPhotos([FromQuery] string keywords,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageNum = 1,
            [FromQuery] int pageSize = 400)
        {
            var dateRange = null as DateRange?;

            if (fromDate != null || toDate != null)
            {
                dateRange = new DateRange(fromDate, toDate);
            }

            IEnumerable<Photo> photos;

            _photoService.SetUserContext(User);

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                photos = await _photoService.GetPhotosByKeywords(keywords, dateRange, pageNum, pageSize);
            }
            else if (dateRange != null)
            {
                photos = await _photoService.GetPhotosByDate(dateRange.Value, pageNum, pageSize);
            }
            else
            {
                return BadRequest();
            }

            var dtos = new List<Dto.Photo>();

            foreach (var photo in photos)
            {
                photo.FileName = GetEncryptedFileName(photo.FileName);
                dtos.Add(new Dto.Photo(photo));
            }

            return Ok(dtos);
        }

        /// <summary>
        /// Deletes a photo and its files.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <returns>An action task.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{photoId}", Name = "DeletePhoto")]
        public async Task<IActionResult> DeletePhoto([FromRoute] int photoId)
        {
            _photoService.SetUserContext(User);
            await _photoService.DeletePhoto(photoId);

            return Ok();
        }

        private string GetEncryptedFileName(string fileName)
        {
            var expiration = DateTime.UtcNow.ToStartOfDay().AddDays(_staticConfig.PhotoExpirationDays);

            return $"{fileName}|{expiration}".Encrypt(_staticConfig.ImageEncryptKey, _staticConfig.ImageEncryptPasscode);
        }
    }
}

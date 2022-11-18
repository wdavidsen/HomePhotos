using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using SCS.HomePhotos.Data;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Core;
using SCS.HomePhotos.Web.Models;

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>Tag services.</summary>
    [Authorize]
    [Route("api/[controller]")]
    public class TagsController : HomePhotosController
    {
        private readonly ILogger<TagsController> _logger;        
        private readonly IPhotoService _photoService;

        /// <summary>Initializes a new instance of the <see cref="TagsController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="photoSevice">The photo service.</param>
        public TagsController(ILogger<TagsController> logger, IPhotoService photoSevice)
        {
            _logger = logger;
            _photoService = photoSevice;
        }

        /// <summary>Gets all tags.</summary>
        /// <returns>A list of tags.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]        
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Dto.Tag>))]
        [HttpGet]
        [Authorize(Policy = "Readers")]
        public async Task<IActionResult> Get()
        {
            _photoService.SetUserContext(User);
            var tags = await _photoService.GetTags(true);

            var dtos = new List<Dto.Tag>();

            foreach (var tag in tags)
            {
                dtos.Add(new Dto.Tag(tag));
            }

            return Ok(dtos);
        }

        /// <summary>Searches the tags.</summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="fromDate">The optional date to start search.</param>
        /// <param name="toDate">The optional date to end search.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of tags.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Dto.Tag>))]
        [HttpGet("search", Name = "SearchTags")]
        public async Task<IActionResult> SearchTags([FromQuery] string keywords,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageNum = 1,
            [FromQuery] int pageSize = 200)
        {
            var dateRange = null as DateRange?;

            if (fromDate != null || toDate != null)
            {
                dateRange = new DateRange(fromDate, toDate);
            }

            IEnumerable<Tag> tags;

            _photoService.SetUserContext(User);

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                tags = await _photoService.GetTagsByKeywords(keywords, dateRange, pageNum, pageSize);
            }
            else if (dateRange != null)
            {
                tags = await _photoService.GetTagsByDate(dateRange.Value, pageNum, pageSize);
            }
            else
            {
                return BadRequest();
            }

            var dtos = new List<Dto.Tag>();

            foreach (var tag in tags)
            {
                dtos.Add(new Dto.Tag(tag));
            }

            return Ok(dtos);
        }

        /// <summary>Merges tags.</summary>
        /// <param name="mergeInfo">The merge information.</param>
        /// <returns>Final merged tag.</returns>
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.Tag))]
        [Authorize(Policy = "Contributers")]
        [HttpPut("merge", Name = "MergeTags")]
        public async Task<IActionResult> MergeTags([FromBody] TagMergeInfo mergeInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }
            
            try
            {
                _photoService.SetUserContext(User);
                var finalTag = await _photoService.MergeTags(mergeInfo.NewTagName, mergeInfo.SourceTagIds);
                return Ok(new Dto.Tag(finalTag));
            }
            catch (AccessException ex)
            {
                return StatusCode(403, new ProblemModel { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ProblemModel { Message = ex.Message });
            }
        }

        /// <summary>Copies a tag.</summary>
        /// <param name="copyInfo">The copy information.</param>
        /// <returns>The new tag.</returns>
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.Tag))]
        [Authorize(Policy = "Contributers")]
        [HttpPut("copy", Name = "CopyTags")]
        public async Task<IActionResult> CopyTag([FromBody] TagCopyInfo copyInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            try
            {
                _photoService.SetUserContext(User);
                var newTag = await _photoService.CopyTags(copyInfo.NewTagName, copyInfo.SourceTagId, copyInfo.OwnerId);

                return Ok(new Dto.Tag(newTag));
            }
            catch (AccessException ex)
            {
                return StatusCode(403, new ProblemModel { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ProblemModel { Message = ex.Message });
            }
        }

        /// <summary>Gets specified photos to tag.</summary>
        /// <param name="photoIds">The photo ids.</param>
        /// <returns>Batch tag info.</returns>
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BatchSelectTags))]
        [Authorize(Policy = "Contributers")]
        [HttpPost("batchTag", Name = "GetPhotosToTag")]
        public async Task<IActionResult> GetPhotosToTag([FromBody] int[] photoIds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }
            _photoService.SetUserContext(User);
            var photoTags = await _photoService.GetTagsAndPhotos(photoIds);

            return Ok(new BatchSelectTags(photoIds, photoTags));
        }

        /// <summary>Tags the photos.</summary>
        /// <param name="updateTags">The updated tags.</param>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = "Contributers")]
        [HttpPut("batchTag", Name = "TagPhotos")]
        public async Task<IActionResult> TagPhotos([FromBody] BatchUpdateTags updateTags)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            try
            {
                _photoService.SetUserContext(User);
                await _photoService.UpdatePhotoTags(updateTags.PhotoIds, updateTags.GetAddedTagNames(), updateTags.GetRemovedTagIds());

                return Ok();
            }
            catch (AccessException ex)
            {
                return StatusCode(403, new ProblemModel { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ProblemModel { Message = ex.Message });
            }            
        }

        /// <summary>Adds a tag.</summary>
        /// <param name="tag">The tag to add.</param>        
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemModel))]        
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.Tag))]
        [Authorize(Policy = "Contributers")]
        [HttpPost(Name = "AddTag")]
        public async Task<IActionResult> AddTag([FromBody] Dto.Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            try
            {
                _photoService.SetUserContext(User);
                var tagEntity = await _photoService.SaveTag(tag.ToModel());

                return Ok(new Dto.Tag(tagEntity));
            }
            catch (AccessException ex)
            {
                return StatusCode(403, new ProblemModel { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ProblemModel { Message = ex.Message });
            }
        }

        /// <summary>Updates a tag.</summary>
        /// <param name="tag">The tag to update.</param>
        /// <returns>The updated tag.</returns>        
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemModel))]        
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.Tag))]
        [Authorize(Policy = "Contributers")]
        [HttpPut(Name = "UpdateTag")]
        public async Task<IActionResult> UpdateTag([FromBody] Dto.Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            try
            {
                _photoService.SetUserContext(User);
                var tagEntity = await _photoService.SaveTag(tag.ToModel());

                return Ok(new Dto.Tag(tagEntity));
            }
            catch (AccessException ex)
            {
                return BadRequest(new ProblemModel { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ProblemModel { Message = ex.Message });
            }
        }

        /// <summary>Deletes a tag.</summary>
        /// <param name="tagId">The tag id.</param>
        [Authorize(Policy = "Contributers")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{tagId}", Name = "DeleteTag")]
        public async Task<IActionResult> DeleteTag([FromRoute] int tagId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            try
            {
                _photoService.SetUserContext(User);
                await _photoService.DeleteTag(tagId);
            }
            catch (AccessException ex)
            {
                return BadRequest(new ProblemModel { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Failed to delete tag {tagId}.", tagId);
                return BadRequest(new ProblemModel { Message = $"Failed to delete tag {tagId}." });
            }

            return Ok();
        }
    }
}

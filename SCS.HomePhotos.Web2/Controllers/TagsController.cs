using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>Tag services.</summary>
    [Authorize]
    [Route("api/[controller]")]
    public class TagsController : HomePhotosController
    {
        private readonly ILogger<TagsController> _logger;
        private readonly IPhotoService _photoSevice;

        /// <summary>Initializes a new instance of the <see cref="TagsController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="photoSevice">The photo service.</param>
        public TagsController(ILogger<TagsController> logger, IPhotoService photoSevice)
        {
            _logger = logger;
            _photoSevice = photoSevice;
        }

        /// <summary>Gets all tags.</summary>
        /// <returns>A list of tags.</returns>
        [HttpGet]
        [Authorize(Policy = "Readers")]
        public async Task<IActionResult> Get()
        {
            var tags = await _photoSevice.GetTags(true);

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

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                tags = await _photoSevice.GetTagsByKeywords(keywords, dateRange, pageNum, pageSize);
            }
            else if (dateRange != null)
            {
                tags = await _photoSevice.GetTagsByDate(dateRange.Value, pageNum, pageSize);
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
        [Authorize(Policy = "Contributers")]        
        [HttpPut("merge", Name = "MergeTags")]
        public async Task<IActionResult> MergeTags([FromBody] TagMergeInfo mergeInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            var finalTag = await _photoSevice.MergeTags(mergeInfo.NewTagName, mergeInfo.SourceTagIds);

            return Ok(new Dto.Tag(finalTag));
        }

        /// <summary>Copies a tag.</summary>
        /// <param name="copyInfo">The copy information.</param>
        /// <returns>The new tag.</returns>
        [Authorize(Policy = "Contributers")]
        [HttpPut("copy", Name = "CopyTags")]
        public async Task<IActionResult> CopyTag([FromBody] TagCopyInfo copyInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            var newTag = await _photoSevice.CopyTags(copyInfo.NewTagName, copyInfo.SourceTagId);

            return Ok(new Dto.Tag(newTag));
        }

        /// <summary>Gets specified photos to tag.</summary>
        /// <param name="photoIds">The photo ids.</param>
        /// <returns>Batch tag info.</returns>
        [Authorize(Policy = "Contributers")]
        [HttpPost("batchTag", Name = "GetPhotosToTag")]
        public async Task<IActionResult> GetPhotosToTag([FromBody] int[] photoIds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            var photoTags = await _photoSevice.GetTagsAndPhotos(photoIds);

            return Ok(new BatchSelectTags(photoIds, photoTags));
        }

        /// <summary>Tags the photos.</summary>
        /// <param name="updateTags">The updated tags.</param>
        [Authorize(Policy = "Contributers")]        
        [HttpPut("batchTag", Name = "TagPhotos")]
        public async Task<IActionResult> TagPhotos([FromBody] BatchUpdateTags updateTags)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            await _photoSevice.UpdatePhotoTags(updateTags.PhotoIds, updateTags.GetAddedTagNames(), updateTags.GetRemovedTagIds());

            return Ok();
        }

        /// <summary>Adds a tag.</summary>
        /// <param name="tag">The tag to add.</param>
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
                var tagEntity = await _photoSevice.SaveTag(tag.ToModel());

                return Ok(new Dto.Tag(tagEntity));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>Updates a tag.</summary>
        /// <param name="tag">The tag to update.</param>
        /// <returns>The updated tag.</returns>
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
                var tagEntity = await _photoSevice.SaveTag(tag.ToModel());

                return Ok(new Dto.Tag(tagEntity));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
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
                await _photoSevice.DeleteTag(tagId);
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

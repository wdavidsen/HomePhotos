using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TagsController : HomePhotosController
    {
        private readonly ILogger<TagsController> _logger;
        private readonly IPhotoService _photoSevice;

        public TagsController(ILogger<TagsController> logger, IPhotoService photoSevice)
        {
            _logger = logger;
            _photoSevice = photoSevice;
        }

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

        [HttpGet("search", Name = "SearchTags")]
        public async Task<IActionResult> SearchTags([FromQuery] string keywords, [FromQuery] int pageNum = 1, [FromQuery] int pageSize = 200)
        {
            var tags = await _photoSevice.GetTagsByKeywords(keywords, pageNum, pageSize);

            var dtos = new List<Dto.Tag>();

            foreach (var tag in tags)
            {
                dtos.Add(new Dto.Tag(tag));
            }

            return Ok(dtos);
        }

        [Authorize(Policy = "Contributers")]
        [ValidateAntiForgeryToken]
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

        [Authorize(Policy = "Contributers")]
        [ValidateAntiForgeryToken]
        [HttpPut("copy", Name = "CopyTags")]
        public async Task<IActionResult> CopyTags([FromBody] TagCopyInfo copyInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            var newTag = await _photoSevice.CopyTags(copyInfo.NewTagName, copyInfo.SourceTagId);

            return Ok(new Dto.Tag(newTag));
        }

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

        [Authorize(Policy = "Contributers")]
        [ValidateAntiForgeryToken]
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

        [Authorize(Policy = "Contributers")]
        [ValidateAntiForgeryToken]
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

        [Authorize(Policy = "Contributers")]
        [ValidateAntiForgeryToken]
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

        [Authorize(Policy = "Contributers")]
        [ValidateAntiForgeryToken]
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
                return BadRequest(new { ex.Message });
            }

            return Ok();
        }
    }
}

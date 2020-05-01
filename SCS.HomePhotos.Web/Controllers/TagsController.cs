using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Models;
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

        [Authorize(Policy = "AdminsOnly")]
        [HttpPut("merge", Name = "MergeTags")]
        public async Task<IActionResult> MergeTags([FromBody]TagMergeInfo mergeInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _photoSevice.MergeTags(mergeInfo.NewTagName, mergeInfo.SourceTagIds);

            return Ok();
        }

        [Authorize(Policy = "AdminsOnly")]
        [HttpPut("copy", Name = "CopyTags")]
        public async Task<IActionResult> CopyTags([FromBody]TagCopyInfo copyInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _photoSevice.CopyTags(copyInfo.NewTagName, copyInfo.SourceTagId);

            return Ok();
        }

        [Authorize(Policy = "AdminsOnly")]
        [HttpPost("batchTag", Name = "GetPhotosToTag")]
        public async Task<IActionResult> GetPhotosToTag([FromBody]int[] photoIds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var photoTags = await _photoSevice.GetTagsAndPhotos(photoIds);

            return Ok(new BatchSelectTags(photoIds, photoTags));
        }

        [Authorize(Policy = "AdminsOnly")]
        [HttpPut("batchTag", Name = "TagPhotos")]
        public async Task<IActionResult> TagPhotos([FromBody]BatchUpdateTags updateTags)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _photoSevice.UpdatePhotoTags(updateTags.PhotoIds, updateTags.GetAddedTagNames(), updateTags.GetRemovedTagIds());

            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
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
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SCS.HomePhotos.Data;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Core;
using SCS.HomePhotos.Web.Models;

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>File exclusion services.</summary>
    [Authorize]
    [Route("api/[controller]")]
    public class FileExclusionController : HomePhotosController
    {
        private readonly ILogger<FileExclusionController> _logger;
        private readonly IFileExclusionData _fileExclusionData;
        private readonly IDynamicConfig _dynamicConfiguration;

        /// <summary>Initializes a new instance of the <see cref="FileExclusionController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="fileExclusionData">The photo service.</param>
        /// <param name="dynamicConfiguration">The dynamic configuration.</param>
        public FileExclusionController(ILogger<FileExclusionController> logger, IFileExclusionData fileExclusionData, IDynamicConfig dynamicConfiguration)
        {
            _logger = logger;
            _fileExclusionData = fileExclusionData;
            _dynamicConfiguration = dynamicConfiguration;
        }

        /// <summary>Gets all file exclusions.</summary>
        /// <returns>A list of file exclusions.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Dto.FileExclusion>))]
        [HttpGet]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> Get()
        {
            var fileExclusions = await _fileExclusionData.GetListAsync();

            var dtos = new List<Dto.FileExclusion>();

            foreach (var fe in fileExclusions)
            {
                dtos.Add(new Dto.FileExclusion(fe, _dynamicConfiguration));
            }

            dtos = dtos.OrderBy(d => d.FullPath).ToList();

            return Ok(dtos);
        }

        /// <summary>Gets a file exclusion by id.</summary>
        /// <param name="exclusion">The file exclusion to add.</param>    
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemModel))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Dto.FileExclusion))]
        [HttpPost]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> Post([FromBody] Dto.FileExclusion exclusion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemModel(ModelState));
            }

            try
            {
                var entity = exclusion.ToModel(_dynamicConfiguration);
                entity.FileExclusionId = await _fileExclusionData.InsertAsync(entity);

                return CreatedAtAction("GetSingle", new Dto.FileExclusion(entity, _dynamicConfiguration));
            }
            catch (FormatException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>Deletes a file exclusion.</summary>
        /// <param name="fileExclusionId">The file exclusion id.</param>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{fileExclusionId}")]
        public async Task<IActionResult> Delete([FromRoute] int fileExclusionId)
        {
            await _fileExclusionData.DeleteAsync(fileExclusionId);

            return Ok();
        }

        /// <summary>Deletes a file exclusion.</summary>
        /// <param name="fileExclusionId">The file exclusion id.</param>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dto.FileExclusion))]
        [HttpGet("{fileExclusionId}", Name = "GetSingle")]
        public async Task<IActionResult> GetSingle([FromRoute] int fileExclusionId)
        {
            var entity = await _fileExclusionData.GetAsync(fileExclusionId);

            return Ok(new Dto.FileExclusion(entity, _dynamicConfiguration));
        }
    }
}

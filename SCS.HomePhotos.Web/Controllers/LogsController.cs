using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>Log services.</summary>
    [Authorize(Policy = "Admins")]
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : HomePhotosController
    {
        private readonly ILogger<LogsController> _logger;
        private readonly IAdminLogService _adminLogService;

        /// <summary>Initializes a new instance of the <see cref="LogsController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="adminLogService">The admin log service.</param>
        public LogsController(ILogger<LogsController> logger, IAdminLogService adminLogService)
        {
            _logger = logger;
            _adminLogService = adminLogService;
        }

        /// <summary>Gets the latest log entries.</summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="category">The category.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <param name="sortDescending">if set to <c>true</c> sort descending.</param>
        /// <returns>Log entries.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataList<Dto.LogEntry>))]
        [HttpGet("latest", Name = "GetLatestLogs")]
        public async Task<IActionResult> GetLatestLogs(
            [FromQuery] int pageSize = 200,
            [FromQuery] int pageNum = 1,
            [FromQuery] LogCategory? category = null,
            [FromQuery] LogSeverity? severity = null,
            [FromQuery] string sortBy = "Timestamp",
            [FromQuery] bool sortDescending = true)
        {
            var pageInfo = new PageInfo
            {
                SortBy = sortBy,
                SortDescending = sortDescending,
                PageSize = pageSize,
                PageNum = pageNum
            };

            var result = await _adminLogService.GetLatestLogEntries(pageInfo, category, severity);

            var dtos = new List<Dto.LogEntry>();

            foreach (var entry in result.Data)
            {
                dtos.Add(new Dto.LogEntry(entry));
            }

            return Ok(new DataList<Dto.LogEntry>(dtos, result.PageInfo));
        }
    }
}
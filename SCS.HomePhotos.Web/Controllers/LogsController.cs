using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Data;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Controllers
{
    [Authorize(Policy = "Admins")]
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : HomePhotosController
    {
        private readonly ILogger<LogsController> _logger;
        private readonly IAdminLogService _adminLogService;

        public LogsController(ILogger<LogsController> logger, IAdminLogService adminLogService)
        {
            _logger = logger;
            _adminLogService = adminLogService;
        }

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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>Upload services.</summary>
    [Authorize]
    [Authorize(Roles = "Admin,Contributer")]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : HomePhotosController
    {
        private readonly ILogger<UploadController> _logger;
        private readonly IImageService _imageService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IAdminLogService _adminLogService;
        private readonly IUploadTracker _uploadTracker;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadController"/> class.
        /// </summary>
        public UploadController(ILogger<UploadController> logger, IImageService imageService, IFileUploadService fileUploadService,
            IAdminLogService adminLogService, IUploadTracker uploadTracker)
        {
            _logger = logger;
            _imageService = imageService;
            _fileUploadService = fileUploadService;
            _adminLogService = adminLogService;
            _uploadTracker = uploadTracker;
        }

        // [DisableRequestSizeLimit]


        /// <summary>Receives an image upload.</summary>
        /// <param name="formdata">The form data containing images and tag info.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("imageUpload")]
        [RequestSizeLimit(20_000_000)] // 20 MB        
        [SuppressMessage("Security", "SCS0018:Path traversal: injection possible in {1} argument passed to '{0}'", Justification = "Invalid characters are being checked in file name prior to passing it to steam.")]
        public async Task<IActionResult> ImageUpload(IFormCollection formdata)
        {
            var acceptedExtensions = new string[] { "JPG", "JPEG", "PNG", "GIF" };

            var files = HttpContext.Request.Form.Files;
            var tags = new List<string>();

            if (formdata.ContainsKey("tagList") && formdata["tagList"].ToString().Length > 0)
            {
                tags = formdata["tagList"].ToString().Split(',').ToList();
            }

            foreach (var file in files)
            {
                string fileName = "";
                string filePath = "";

                try
                {
                    fileName = file.GetFileName();

                    var extension = Path.GetExtension(fileName);
                    var tempDir = Path.GetTempPath();

                    #region Validations

                    // check for invalid characters
                    if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    {
                        return BadRequest();
                    }

                    // file name should not exceed 255 characters
                    if (fileName.Length > 255)
                    {
                        return BadRequest();
                    }

                    // check for valid extensions
                    if (!Constants.AcceptedExtensions.Any(e => e == extension.ToUpper()))
                    {
                        return BadRequest();
                    }

                    // check image header bytes
                    using (var sourceStream = file.OpenReadStream())
                    {
                        if (!ImageValidationHelper.ValidateImageHeaders(sourceStream))
                        {
                            return BadRequest();
                        }
                    }
                    #endregion

                    filePath = Path.Combine(tempDir, fileName);
                    await _fileUploadService.CopyFile(file, filePath, FileMode.Create);

                    var user = User.Identity.Name;
                    tags.Add($"{user} Upload");

                    var cachePath = await _imageService.QueueMobileResize(user, filePath, tags.ToArray());
                    LogUpload(User.Identity.Name);

                    _uploadTracker.AddUpload(User.Identity.Name, filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to receive uploaded file: {fileName}.", fileName);
                    throw;
                }
            }

            return Ok();
        }

        private void LogUpload(string username)
        {
            var msg = $"User {username} uploaded photos.";

            var entry = new LogEntry
            {
                Message = msg,
                Category = LogCategory.Upload,
                Severity = LogSeverity.Neutral,
                Timestamp = DateTime.Now
            };

            _adminLogService.AddLogEntry(entry, TimeSpan.FromMinutes(30));
        }
    }
}
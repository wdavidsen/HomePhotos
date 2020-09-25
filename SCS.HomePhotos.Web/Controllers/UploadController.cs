using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>
    /// Receives uploaded files.
    /// </summary>
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : HomePhotosController
    {
        private readonly ILogger<UploadController> _logger;
        private readonly IImageService _imageService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IAdminLogService _adminLogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadController"/> class.
        /// </summary>
        public UploadController(ILogger<UploadController> logger, IImageService imageService, IFileUploadService fileUploadService,
            IAdminLogService adminLogService)
        {
            _logger = logger;
            _imageService = imageService;
            _fileUploadService = fileUploadService;
            _adminLogService = adminLogService;
        }

        // [DisableRequestSizeLimit]

        /// <summary>
        /// Receives one or more image files and returns their thumbnail URLs.
        /// </summary>
        /// <param name="formdata">The uploaded files and form data.</param>
        [HttpPost]
        [Route("imageUpload")]
        [RequestSizeLimit(20_000_000)] // 20 MB        
        [SuppressMessage("Security", "SCS0018:Path traversal: injection possible in {1} argument passed to '{0}'", Justification = "Invalid characters are being checked in file name prior to passing it to steam.")]
        public async Task<IActionResult> ImageUpload(IFormCollection formdata)
        {
            var acceptedExtensions = new string[] { "JPG", "JPEG", "PNG", "GIF" };

            var files = HttpContext.Request.Form.Files;
            var tags = (formdata.ContainsKey("tagList") && formdata["tagList"].Count > 0) ? formdata["tagList"].ToList() : new List<string>();

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
                    if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                    {
                        return BadRequest();
                    }

                    // check for valid extensions
                    if (!acceptedExtensions.Any(e => e == extension.TrimStart('.').ToUpper()))
                    {
                        return BadRequest();
                    }

                    // check image header bytes
                    //using (var sourceStream = file.OpenReadStream())
                    //{
                    //    if (!ImageValidationHelper.ValidateImageHeaders(sourceStream))
                    //    {
                    //        return BadRequest();
                    //    }
                    //}

                    // file name should not exceed 255 characters
                    if (fileName.Length > 255)
                    {
                        throw new InvalidOperationException($"Uploaded file name too long. Length: {fileName.Length}.");
                    }
                    #endregion

                    filePath = Path.Combine(tempDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    tags.Add($"{User.Identity.Name} Upload");

                    var cachePath = await _imageService.QueueMobileResize(filePath, false, tags.ToArray());
                    LogUpload(User.Identity.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to receive uploaded file: {fileName}");
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
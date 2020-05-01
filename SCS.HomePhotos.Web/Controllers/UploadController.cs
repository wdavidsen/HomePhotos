using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadController"/> class.
        /// </summary>
        public UploadController(ILogger<UploadController> logger, IImageService imageService, IFileUploadService fileUploadService)
        {
            _logger = logger;
            _imageService = imageService;
            _fileUploadService = fileUploadService;
        }

        // [DisableRequestSizeLimit]

        /// <summary>
        /// Receives one or more image files and returns their thumbnail URLs.
        /// </summary>
        /// <param name="files">The uploaded files.</param>
        /// <returns>Image file status and thumbnail URL.</returns>
        [HttpPost]
        [Route("imageUpload")]
        [RequestSizeLimit(20_000_000)] // 20 MB        
        [SuppressMessage("Security", "SCS0018:Path traversal: injection possible in {1} argument passed to '{0}'", Justification = "Invalid characters are being checked in file name prior to passing it to steam.")]
        public async Task<IActionResult> ImageUpload(List<IFormFile> files)
        {
            var acceptedExtensions = new string[] { "JPG", "PNG", "GIF" };

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

                    var cachePath = await _imageService.QueueMobileResize(filePath, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to receive uploaded file: {fileName}");
                    throw;
                }
            }

            return Ok();
        }
    }
}
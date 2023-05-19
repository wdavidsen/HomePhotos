using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Web.Filters;

using System.Diagnostics.CodeAnalysis;

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>Upload services.</summary>        
    [Authorize(Roles = "Admin,Contributor")]
    [UserExists]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : HomePhotosController
    {
        private readonly ILogger<UploadController> _logger;
        private readonly IImageService _imageService;
        private readonly IUserData _userData;
        private readonly IFileUploadService _fileUploadService;
        private readonly IAdminLogService _adminLogService;
        private readonly IUploadTracker _uploadTracker;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="imageService">The image service.</param>
        /// <param name="userData">The user data.</param>
        /// <param name="fileUploadService">The file upload service.</param>
        /// <param name="adminLogService">The admin log service.</param>
        /// <param name="uploadTracker">The upload tracker.</param>
        public UploadController(ILogger<UploadController> logger, IImageService imageService, IUserData userData, IFileUploadService fileUploadService,
            IAdminLogService adminLogService, IUploadTracker uploadTracker)
        {
            _logger = logger;
            _imageService = imageService;
            _userData = userData;
            _fileUploadService = fileUploadService;
            _adminLogService = adminLogService;
            _uploadTracker = uploadTracker;
        }

        /// <summary>Receives an image upload.</summary>
        /// <param name="formdata">The form data containing images and tag info.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("imageUpload")]
        [RequestSizeLimit(20_000_000)] // 20 MB        
        [SuppressMessage("Security", "SCS0018:Path traversal: injection possible in {1} argument passed to '{0}'", Justification = "Invalid characters are being checked in file name prior to passing it to stream.")]
        public async Task<IActionResult> ImageUpload(IFormCollection formdata)
        {
            var acceptedExtensions = new string[] { "JPG", "JPEG", "PNG", "GIF" };

            var files = HttpContext.Request.Form.Files;
            var tags = new List<Tag>();

            var currentUser = await _userData.GetUser(User.Identity.Name);

            if (formdata.ContainsKey("tagList") && formdata["tagList"].ToString().Length > 0)
            {
                PopulateUploadTags(formdata, tags);
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

                    var cachePath = await _imageService.QueueMobileResize(currentUser, filePath, tags);
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

        private static void PopulateUploadTags(IFormCollection formdata, List<Tag> tags)
        {
            var list = formdata["tagList"].ToString().Split(',');

            foreach (var item in list)
            {
                var idAndTag = item.Split('^');

                if (idAndTag.Length > 1 && int.TryParse(idAndTag[0], out var userId))
                {
                    tags.Add(new Tag
                    {
                        UserId = userId > 0 ? userId : null,
                        TagName = idAndTag[1]
                    });
                }
                else
                {
                    tags.Add(new Tag
                    {
                        UserId = null,
                        TagName = idAndTag[0]
                    });
                }
            }
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
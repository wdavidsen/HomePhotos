﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>Photo services.</summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PhotosController : HomePhotosController
    {
        private readonly ILogger<PhotosController> _logger;
        private readonly IPhotoService _photoSevice;
        private readonly IStaticConfig _staticConfig;

        /// <summary>Initializes a new instance of the <see cref="PhotosController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="photoSevice">The photo sevice.</param>
        /// <param name="staticConfig">The static configuration.</param>
        public PhotosController(ILogger<PhotosController> logger, IPhotoService photoSevice, IStaticConfig staticConfig)
        {
            _logger = logger;
            _photoSevice = photoSevice;
            _staticConfig = staticConfig;
        }

        /// <summary>Gets the latest photos.</summary>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of photos.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Dto.Photo>))]
        [HttpGet("latest", Name = "GetLatestPhotos")]
        public async Task<IActionResult> GetLatestPhotos([FromQuery] int pageNum = 1, [FromQuery] int pageSize = 400)
        {
            var photos = await _photoSevice.GetLatestPhotos(pageNum, pageSize);

            var dtos = new List<Dto.Photo>();

            foreach (var photo in photos)
            {
                photo.FileName = GetEncryptedFileName(photo.FileName);
                dtos.Add(new Dto.Photo(photo));
            }

            return Ok(dtos);
        }

        /// <summary>Gets photos by tag.</summary>
        /// <param name="tag">The tag.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of photos.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Dto.Photo>))]
        [HttpGet("byTag", Name = "GetPhotosByTag")]
        public async Task<IActionResult> GetPhotosByTag([FromQuery] string tag, [FromQuery] int pageNum = 1, [FromQuery] int pageSize = 400)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return BadRequest();
            }

            var photos = await _photoSevice.GetPhotosByTag(new string[] { tag }, pageNum, pageSize);

            var dtos = new List<Dto.Photo>();

            foreach (var photo in photos)
            {
                photo.FileName = GetEncryptedFileName(photo.FileName);
                dtos.Add(new Dto.Photo(photo));
            }

            return Ok(dtos);
        }

        /// <summary>Searches photos.</summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A list of photos.</returns>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Dto.Photo>))]
        [HttpGet("search", Name = "SearchPhotos")]
        public async Task<IActionResult> SearchPhotos([FromQuery] string keywords, [FromQuery] int pageNum = 1, [FromQuery] int pageSize = 400)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                return BadRequest();
            }

            var photos = await _photoSevice.GetPhotosByKeywords(keywords, pageNum, pageSize);

            var dtos = new List<Dto.Photo>();

            foreach (var photo in photos)
            {             
                photo.FileName = GetEncryptedFileName(photo.FileName);
                dtos.Add(new Dto.Photo(photo));
            }

            return Ok(dtos);
        }

        private string GetEncryptedFileName(string fileName)
        {
            var expiration = DateTime.UtcNow.ToStartOfDay().AddDays(_staticConfig.PhotoExpirationDays);
            return $"{fileName}|{expiration}".Encrypt(_staticConfig.ImagePasscode);
        }
    }
}

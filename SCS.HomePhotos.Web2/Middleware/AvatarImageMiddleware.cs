using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Middleware
{
    /// <summary>
    /// Avaitar image middleware.
    /// </summary>
    public class AvatarImageMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvatarImageMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        public AvatarImageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes the asynchronous.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="dynamicConfig">The dynamic configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="env">The environment.</param>
        /// <exception cref="System.IO.FileNotFoundException">Avatar image not found at {tempImagePath}.</exception>
        /// <returns>A void task.</returns>
        public async Task InvokeAsync(HttpContext httpContext, IDynamicConfig dynamicConfig, ILogger<AvatarImageMiddleware> logger, IWebHostEnvironment env)
        {
            var request = httpContext.Request;

            if (request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase)                
                && httpContext.Request.Path.Value.StartsWith(Constants.AvatarRoute, StringComparison.InvariantCultureIgnoreCase))
            {
                var imagePath = Path.Combine(env.WebRootPath.TrimEnd('/', '\\'), Constants.AvatarDefaultFile);

                try
                {
                    var extension = Path.GetExtension(httpContext.Request.Path.Value.TrimStart('.')).ToUpper();

                    if (Path.HasExtension(httpContext.Request.Path.Value) && Constants.AcceptedExtensions.Contains(extension))
                    {
                        var imageName = Path.GetFileName(httpContext.Request.Path.Value);
                        var tempImagePath = Path.Combine(dynamicConfig.CacheFolder.TrimEnd('/', '\\'), Constants.AvatarFolder, imageName);

                        if (!File.Exists(tempImagePath))
                        {
                            throw new FileNotFoundException($"Avatar image not found at {tempImagePath}.");
                        }
                        imagePath = tempImagePath;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to serve avatar image");                    
                }                

                httpContext.Response.Headers.Add("Cache-Control", "Public");
                httpContext.Response.Headers.Add("Expires", DateTime.UtcNow.AddDays(30).ToString("u"));

                await httpContext.Response.SendFileAsync(imagePath);
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}

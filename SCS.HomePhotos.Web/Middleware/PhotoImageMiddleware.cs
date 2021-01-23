using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Middleware
{
    public class PhotoImageMiddleware
    {
        private readonly RequestDelegate _next;

        public PhotoImageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Middleware entry point.
        /// </summary>
        /// <param name="httpContext">The request.</param>
        /// <param name="dynamicConfig">The dynamic configuration.</param>
        /// <param name="staticConfig">The static configuration.</param>
        /// <returns>A void task.</returns>
        public async Task InvokeAsync(HttpContext httpContext, IDynamicConfig dynamicConfig, IStaticConfig staticConfig)
        {
            var request = httpContext.Request;

            if (request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase)
                    && httpContext.Request.Path.Value.StartsWith(Constants.CacheRoute, StringComparison.InvariantCultureIgnoreCase))
            {
                var folderAndFileInfo = httpContext.Request.Path.Value.Substring(Constants.CacheRoute.Length).Trim('/').Split('/');
                var size = httpContext.Request.Query.ContainsKey("type") ? httpContext.Request.Query["type"].ToString().ToLower() : "thumb";

                switch (size)
                {
                    case "thumb":
                    case "thumbnail":
                        size = "thumb";
                        break;
                    case "small":
                    case "sm":
                        size = "small";
                        break;
                    case "full":
                    case "large":
                    case "lg":
                        size = "full";
                        break;
                }

                httpContext.Response.Headers.Add("Cache-Control", "Private");
                httpContext.Response.Headers.Add("Expires", DateTime.UtcNow.AddDays(staticConfig.PhotoExpirationDays).ToString("u"));

                var folder = folderAndFileInfo[0];
                var fileInfo = folderAndFileInfo[1].Decrypt(staticConfig.ImagePasscode);
                var fileInfoParts = fileInfo.Split('|');
                var file = fileInfoParts[0];
                var expiration = DateTime.Parse(fileInfoParts[1]);

                if (expiration < DateTime.UtcNow.ToStartOfDay())
                {
                    httpContext.Response.StatusCode = 400;
                    return;
                }

                var cachePath = Path.Combine(dynamicConfig.CacheFolder.TrimEnd('/', '\\'), folder, size, file);
                await httpContext.Response.SendFileAsync(cachePath);
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}

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
        /// <returns>A void task.</returns>
        public async Task InvokeAsync(HttpContext httpContext, IDynamicConfig dynamicConfig)
        {
            var request = httpContext.Request;

            if (request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase)
                    && httpContext.Request.Path.Value.StartsWith(Constants.CacheRoute, StringComparison.InvariantCultureIgnoreCase))
            {
                var folderAndFile = httpContext.Request.Path.Value.Substring(Constants.CacheRoute.Length).Trim('/').Split('/');
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

                var cachePath = Path.Combine(dynamicConfig.CacheFolder.TrimEnd('/', '\\'), folderAndFile[0], size, folderAndFile[1]);
                await httpContext.Response.SendFileAsync(cachePath);
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}

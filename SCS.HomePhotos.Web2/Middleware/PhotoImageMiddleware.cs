using Microsoft.AspNetCore.Http;
using System;
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
            if (httpContext.Request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase)
                    && httpContext.Request.Path.Value.StartsWith(Constants.CacheRoute, StringComparison.InvariantCultureIgnoreCase))
            {
                var photoFilePath = httpContext.Request.Path.Value.Substring(Constants.CacheRoute.Length);

                httpContext.Response.Headers.Add("Cache-Control", "Private");

                var cachePath = $"{dynamicConfig.CacheFolder.TrimEnd('/', '\\')}/{photoFilePath}";
                await httpContext.Response.SendFileAsync(cachePath);
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}

using Microsoft.AspNetCore.Builder;

namespace SCS.HomePhotos.Web.Middleware
{
    /// <summary>
    /// Middleware extension methods.
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Adds the exception middleware to the request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        public static void UseGloablExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }

        /// <summary>
        /// Adds the photo image middleware to the request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        public static void UsePhotoImageMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<PhotoImageMiddleware>();
        }

        /// <summary>
        /// Adds the antiforgery token for SPA apps.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAntiforgeryToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AntiforgeryTokenMiddleware>();
        }
    }
}

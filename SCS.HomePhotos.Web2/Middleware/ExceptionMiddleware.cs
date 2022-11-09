using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Middleware
{
    /// <summary>
    /// Exception middleware for default exception handling.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Gets the default error message format.
        /// </summary>
        /// <value>
        /// The default error message format.
        /// </value>
        public static string DefaultErrorMessageFormat
        {
            get => "An unexpected error has occurred. Please contact our support and provide them this identifier: {0}.";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware module to execute.</param>
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Middle-ware entry point.
        /// </summary>
        /// <param name="httpContext">The request.</param>
        /// <param name="environment">The hosting environment.</param>
        /// <param name="logFactory">The configured logger to use.</param>
        /// <returns>A void task.</returns>
        public async Task InvokeAsync(HttpContext httpContext, IWebHostEnvironment environment, ILoggerFactory logFactory)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var eventProperties = new StringBuilder(512);
                eventProperties.Append($"; RequestPath={httpContext.Request.Path}");
                eventProperties.Append($"; RequestMethod={httpContext.Request.Method}");
                eventProperties.Append($"; UserName={httpContext?.User.Identity.Name}");
                eventProperties.Append($"; IsAuthenticated={httpContext?.User.Identity.IsAuthenticated}");

                var logger = logFactory.CreateLogger<ExceptionMiddleware>();
                var errorCode = DateTime.Now.Ticks.ToString("x", CultureInfo.CurrentCulture);
                logger.LogError(ex, "Request error. Code={errorCode}{eventProperties}", errorCode, eventProperties);
                var responseMessage = string.Format(CultureInfo.CurrentCulture, DefaultErrorMessageFormat, errorCode);

                if (environment.EnvironmentName.ToUpper().StartsWith("DEV"))
                {
                    responseMessage += " Developer info: " + ex.GetBaseException().Message + ((ex.GetBaseException() != ex) ? (" " + ex.Message) : "");
                }
                await WriteMessageResponse(httpContext, responseMessage);
            }
        }

        private static Task WriteMessageResponse(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(new GlobalErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            }.ToString());
        }
    }
}

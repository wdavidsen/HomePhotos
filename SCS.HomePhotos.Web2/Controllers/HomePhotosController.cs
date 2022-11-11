using Microsoft.AspNetCore.Mvc;

namespace SCS.HomePhotos.Web.Controllers
{
    /// <summary>Base controller.</summary>
    public class HomePhotosController : Controller
    {
        /// <summary>Initializes a new instance of the <see cref="HomePhotosController" /> class.</summary>
        public HomePhotosController()
        {
        }

        /// <summary>Gets the user agent identifier.</summary>
        /// <returns>User agent id.</returns>
        protected string GetAgentIdentifier()
        {
            var ipAddress = HttpContext?.Connection?.RemoteIpAddress;

            if (ipAddress != null)
            {
                return ipAddress.ToString();
            }

            var ipHeader = GetHeader("X-Forwarded-For");

            return $"{ipHeader}:{Request.Headers["User-Agent"].ToString()}";
        }

        /// <summary>Gets a header value.</summary>
        /// <param name="header">The header kwy.</param>
        /// <returns>Header value.</returns>
        protected string GetHeader(string header)
        {
            // https://stackoverflow.com/a/36316189
            if (HttpContext.Request.Headers.TryGetValue(header, out var value))
            {
                return value
                    .ToString()
                    .TrimEnd(',')
                    .Split(',')
                    .Select(s => s.Trim())
                    .FirstOrDefault();
            }
            return null;
        }
    }
}
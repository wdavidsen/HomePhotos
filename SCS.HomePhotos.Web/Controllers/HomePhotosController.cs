using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SCS.HomePhotos.Web.Controllers
{
    public class HomePhotosController : Controller
    {
        public HomePhotosController()
        {            
        }

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
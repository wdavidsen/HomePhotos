using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SCS.HomePhotos.Web.Controllers
{
    // from: https://www.blinkingcaret.com/2018/11/29/asp-net-core-web-api-antiforgery/
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AntiForgeryController : Controller
    {
        private IAntiforgery _antiForgery;

        public AntiForgeryController(IAntiforgery antiForgery)
        {
            _antiForgery = antiForgery;
        }

        [IgnoreAntiforgeryToken]
        public IActionResult Get()
        {
            var tokens = _antiForgery.GetAndStoreTokens(HttpContext);
            Response.Cookies.Append("XSRF-REQUEST-TOKEN", tokens.RequestToken, new CookieOptions
            {
                HttpOnly = false
            });
            return NoContent();
        }
    }
}


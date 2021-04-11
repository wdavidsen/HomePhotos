using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SCS.HomePhotos.Web.Controllers
{
    // from: https://www.blinkingcaret.com/2018/11/29/asp-net-core-web-api-antiforgery/
    /// <summary>Anti-forgery services</summary>    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AntiForgeryController : Controller
    {
        private IAntiforgery _antiForgery;

        /// <summary>Initializes a new instance of the <see cref="AntiForgeryController" /> class.</summary>
        /// <param name="antiForgery">The anti-forgery.</param>
        public AntiForgeryController(IAntiforgery antiForgery)
        {
            _antiForgery = antiForgery;
        }

        /// <summary>Gets an anti-forger cookie.</summary>
        /// <returns>Anti-forger cookie.</returns>
        [IgnoreAntiforgeryToken]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
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


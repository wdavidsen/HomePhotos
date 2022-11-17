using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using Moq;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SCS.HomePhotos.Web.Test.Controllers
{
    public abstract class ControllerTestBase : IDisposable
    {
        public ControllerTestBase() { }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SampleControllerTests()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Sets the controller test context.
        /// </summary>
        /// <param name="controller">The controller to set HttpContext on.</param>
        /// <param name="method">The request method, e.g., GET, POST, etc.</param>
        /// <returns>A controller mock.</returns>
        protected static void SetControllerContext(Controller controller, string method, string userName = null, IFormCollection forms = null)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            var controllerContext = new ControllerContext();

            var authServiceMock = new Mock<IAuthenticationService>();
            var urlHelperFactoryMock = new Mock<IUrlHelperFactory>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            var uriHelperMock = new Mock<IUrlHelper>();

            serviceProviderMock.Setup(m => m.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            serviceProviderMock.Setup(m => m.GetService(typeof(IUrlHelperFactory)))
                .Returns(urlHelperFactoryMock.Object);

            uriHelperMock.Setup(m => m.IsLocalUrl(It.IsAny<string>()))
                .Returns(true);

            ClaimsIdentity identity = null;

            if (userName != null)
            {
                identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                identity.AddClaims(new List<Claim>
                {
                    new Claim(identity.NameClaimType, userName),
                    new Claim(JwtRegisteredClaimNames.Sub, userName),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userName),
                    new Claim(JwtRegisteredClaimNames.Typ, Guid.NewGuid().ToString())
                });
            }

            controllerContext.HttpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object,
                User = identity != null ? new ClaimsPrincipal(identity) : null
            };

            controllerContext.HttpContext.Request.Method = method;

            if (forms != null)
            {
                controllerContext.HttpContext.Request.Form = forms;
            }

            controller.ControllerContext = controllerContext;
            controller.Url = uriHelperMock.Object;
        }
    }
}

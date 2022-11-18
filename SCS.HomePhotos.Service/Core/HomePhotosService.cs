using System.Security.Principal;
using SCS.HomePhotos.Service.Contracts;

namespace SCS.HomePhotos.Service.Core
{
    /// <summary>
    /// Base class for services.
    /// </summary>
    public class HomePhotosService : IHomePhotosService
    {
        /// <summary>
        /// Sets the user context.
        /// </summary>
        /// <param name="user">The user.</param>
        public void SetUserContext(IPrincipal user)
        {
            User = user;
        }

        /// <summary>
        /// Gets the user principal.
        /// </summary>
        /// <value>
        /// The user principal.
        /// </value>
        public IPrincipal User { get; private set; }
    }
}

using System.Security.Principal;

namespace SCS.HomePhotos.Service.Contracts
{
    /// <summary>
    /// Base class for services.
    /// </summary>
    public interface IHomePhotosService
    {
        /// <summary>
        /// Gets the user principal.
        /// </summary>
        /// <value>
        /// The user principal.
        /// </value>
        IPrincipal User { get; }

        /// <summary>
        /// Sets the user context.
        /// </summary>
        /// <param name="user">The user.</param>
        void SetUserContext(IPrincipal user);
    }
}
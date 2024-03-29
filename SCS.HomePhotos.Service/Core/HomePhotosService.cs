﻿using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Data.Core;
using SCS.HomePhotos.Service.Contracts;

using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Core
{
    /// <summary>
    /// Base class for services.
    /// </summary>
    public class HomePhotosService : IHomePhotosService
    {
        private readonly IDynamicConfig _dynamicConfig;
        private readonly IUserData _userData;
        private readonly IUserSettingsData _userSettingsData;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomePhotosService"/> class.
        /// </summary>
        public HomePhotosService()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomePhotosService"/> class.
        /// </summary>
        /// <param name="dynamicConfig">The dynamic configuration.</param>
        /// <param name="userData">The user data.</param>
        /// <param name="userSettingsData">The user settings data.</param>
        public HomePhotosService(IDynamicConfig dynamicConfig, IUserData userData, IUserSettingsData userSettingsData)
        {
            _dynamicConfig = dynamicConfig;
            _userData = userData;
            _userSettingsData = userSettingsData;

            BaselineViewScope = dynamicConfig.UserPhotoViewScope;
        }

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

        /// <summary>
        /// Gets or sets the baseline view scope.
        /// </summary>
        /// <value>
        /// The baseline view scope.
        /// </value>
        public UserPhotoScope BaselineViewScope { get; set; }

        /// <summary>
        /// Gets the view scope.
        /// </summary>        
        /// <param name="desiredOwnerUsername">The desired owner of the photos the current user wishes to view.</param>
        /// <returns>The allowed scope.</returns>
        public async Task<(string OwnerUsername, UserPhotoScope Scope)> GetViewScope(string desiredOwnerUsername)
        {
            (string OwnerUsername, UserPhotoScope Scope) result;

            var allowedUsername = desiredOwnerUsername;
            var (baseScope, isAdmin) = await GetUserScope();
            
            if (BaselineViewScope > baseScope && !isAdmin)
            {
                baseScope = BaselineViewScope;
            }

            if (baseScope != UserPhotoScope.Everything && !string.IsNullOrEmpty(desiredOwnerUsername) && !desiredOwnerUsername.Equals(User.Identity.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                allowedUsername = string.Empty;
            }

            result = (allowedUsername, baseScope);

            return result;
        }

        private async Task<(UserPhotoScope Scope, bool IsAdmin)> GetUserScope()
        {
            var user = await _userData.GetUser(User.Identity.Name);

            if (user == null)
            {
                return (UserPhotoScope.PersonalOnly, false);
            }
            var userSettings = await _userSettingsData.GetSettings(user.UserId.Value);

            return (userSettings.UserScope, user.Role == Model.RoleType.Admin);
        }
    }
}

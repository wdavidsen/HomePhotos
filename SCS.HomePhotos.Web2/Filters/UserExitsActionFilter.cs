using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using SCS.HomePhotos.Data.Contracts;

namespace SCS.HomePhotos.Web.Filters
{
    /// <summary>
    /// Action filter that verifies user identity is also in the database as a user record.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.TypeFilterAttribute" />
    public class UserExistsAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserExistsAttribute"/> class.
        /// </summary>
        public UserExistsAttribute() : base(typeof(InternalUserExistsAttribute)) { }

        /// <summary>
        /// Action filter that verifies user identity is also in the database as a user record.
        /// </summary>
        /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.IAsyncActionFilter" />
        private class InternalUserExistsAttribute : IAsyncActionFilter
        {
            private readonly IUserData _userData;

            /// <summary>
            /// Initializes a new instance of the <see cref="InternalUserExistsAttribute"/> class.
            /// </summary>
            /// <param name="userData">The user data.</param>
            public InternalUserExistsAttribute(IUserData userData)
            {
                _userData = userData;
            }

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext" />.</param>
            /// <param name="next">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate" />. Invoked to execute the next action filter or the action itself.</param>
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var username = context.HttpContext.User?.Identity?.Name;

                if (username == null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                if (await _userData.GetUser(username) == null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                await next();
            }
        }
    }
}

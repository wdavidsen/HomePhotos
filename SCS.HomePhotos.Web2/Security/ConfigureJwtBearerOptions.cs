using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.Security.Claims;

namespace SCS.HomePhotos.Web.Security
{
    internal class ConfigureJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly IOptions<JwtAuthentication> _jwtAuthentication;

        public ConfigureJwtBearerOptions(IOptions<JwtAuthentication> jwtAuthentication)
        {
            _jwtAuthentication = jwtAuthentication ?? throw new ArgumentNullException(nameof(jwtAuthentication));
        }

        public void PostConfigure(string name, JwtBearerOptions options)
        {
            var jwtAuthentication = _jwtAuthentication.Value;

            options.ClaimsIssuer = jwtAuthentication.ValidIssuer;
            options.IncludeErrorDetails = true;
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateActor = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtAuthentication.ValidIssuer,
                ValidAudience = jwtAuthentication.ValidAudience,
                IssuerSigningKey = jwtAuthentication.SymmetricSecurityKey,
                NameClaimType = ClaimTypes.NameIdentifier
            };
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    //if (!context.Request.Path.Value.StartsWith(Constants.RefreshRoute, StringComparison.InvariantCultureIgnoreCase)
                    //    && context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    //{
                    //    context.Response.Headers.Add("token-expired", "true");
                    //}
                    return Task.CompletedTask;
                }
            };
        }
    }
}

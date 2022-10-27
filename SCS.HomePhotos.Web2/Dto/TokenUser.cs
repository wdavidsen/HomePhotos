using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    /// <summary>
    /// A user with their auth token information.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Web.Dto.User" />
    public class TokenUser : User
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenUser"/> class.
        /// </summary>
        public TokenUser() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenUser"/> class using the domain model class and token info.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="token">The user auth token.</param>
        /// <param name="refreshToken">The refresh token.</param>
        public TokenUser(Model.User user, string token = null, string refreshToken = null) : base(user)
        {
            UserId = user.UserId;
            Username = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            EmailAddress = user.EmailAddress;
            AvatarImage = user.AvatarImage;

            Jwt = token;
            RefreshToken = refreshToken;
        }

        /// <summary>
        /// Gets or sets the user JWT.
        /// </summary>
        /// <value>
        /// The user JWT.
        /// </value>
        [Required]
        public string Jwt { get; set; }

        /// <summary>
        /// Gets or sets the user refresh token.
        /// </summary>
        /// <value>
        /// The user refresh token.
        /// </value>
        [Required]
        public string RefreshToken { get; set; }
    }
}

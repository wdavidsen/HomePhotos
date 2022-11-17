using SCS.HomePhotos.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    /// <summary>
    /// Represents a user in the app.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class using the domain model class.
        /// </summary>
        /// <param name="user">The user.</param>
        public User(Model.User user)
        {
            UserId = user.UserId;
            Username = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            EmailAddress = user.EmailAddress;
            AvatarImage = user.AvatarImage;
            Enabled = user.Enabled;            
            Role = user.Role;
            LastLogin = user.LastLogin;
            FailedLoginCount = user.FailedLoginCount;
            MustChangePassword = user.MustChangePassword;
            TagColor = user.TagColor;
        }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int? UserId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the avatar image.
        /// </summary>
        /// <value>
        /// The avatar image.
        /// </value>
        public string AvatarImage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="User"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [Required]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        [Required]
        public RoleType Role { get; set; }

        /// <summary>
        /// Gets or sets the last login.
        /// </summary>
        /// <value>
        /// The last login.
        /// </value>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Gets or sets the failed login count.
        /// </summary>
        /// <value>
        /// The failed login count.
        /// </value>
        public int FailedLoginCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user must change password.
        /// </summary>
        /// <value>
        ///   <c>true</c> if user must change password; otherwise, <c>false</c>.
        /// </value>
        public bool MustChangePassword { get; set; }

        /// <summary>
        /// Gets or sets the color of the tag.
        /// </summary>
        /// <value>
        /// The color of the tag.
        /// </value>
        public string TagColor { get; set; }

        /// <summary>
        /// Converts instance to the domain model.
        /// </summary>
        /// <returns>The domain equivalent instance.</returns>
        public virtual Model.User ToModel()
        {
            return new Model.User
            {
                UserId = UserId,
                UserName = Username,
                FirstName = FirstName,
                LastName = LastName,
                EmailAddress = EmailAddress,
                AvatarImage = AvatarImage,
                Enabled = Enabled,
                Role = Role,
                LastLogin = LastLogin,
                FailedLoginCount = FailedLoginCount,
                MustChangePassword = MustChangePassword,
                TagColor = TagColor
            };
        }
    }
}

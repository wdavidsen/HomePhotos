using SCS.HomePhotos.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    /// <summary>
    /// Account info DTO.
    /// </summary>
    public class AccountInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountInfo"/> class.
        /// </summary>
        public AccountInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountInfo"/> class using the domain model class.
        /// </summary>
        /// <param name="user">The user.</param>
        public AccountInfo(Model.User user)
        {
            UserId = user.UserId;
            Username = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            EmailAddress = user.EmailAddress;
            AvatarImage = user.AvatarImage;
            Admin = user.Role == RoleType.Admin;
            LastLogin = user.LastLogin;
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
        /// Gets or sets a value indicating whether this <see cref="AccountInfo"/> is admin.
        /// </summary>
        /// <value>
        ///   <c>true</c> if admin; otherwise, <c>false</c>.
        /// </value>
        [Required]
        public bool Admin { get; set; }

        /// <summary>
        /// Gets or sets the last login.
        /// </summary>
        /// <value>
        /// The last login.
        /// </value>
        public DateTime? LastLogin { get; set; }

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
                Role = Admin ? RoleType.Admin : RoleType.Reader,
                LastLogin = LastLogin                
            };
        }
    }
}

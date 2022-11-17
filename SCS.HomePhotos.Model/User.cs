using Dapper;
using System;

namespace SCS.HomePhotos.Model
{
    /// <summary>
    /// The role type;
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// The reader role.
        /// </summary>
        Reader = 0,
        /// <summary>
        /// The contributer role.
        /// </summary>
        Contributer = 1,
        /// <summary>
        /// The admin role.
        /// </summary>
        Admin = 2
    }

    /// <summary>
    /// The user entity.
    /// </summary>
    [Table("User")]
    public class User
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="user">The user to copy.</param>
        public User(Model.User user)
        {
            UserId = user.UserId;
            UserName = user.UserName;
            PasswordHash = user.PasswordHash;
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
        [Key]
        public int? UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        /// <value>
        /// The password hash.
        /// </value>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
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
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public RoleType Role { get; set; }

        /// <summary>
        /// Gets or sets the last login timestamp.
        /// </summary>
        /// <value>
        /// The last login timestamp.
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
        /// Gets or sets a value indicating whether password must change.
        /// </summary>
        /// <value>
        ///   <c>true</c> if must change password; otherwise, <c>false</c>.
        /// </value>
        public bool MustChangePassword { get; set; }

        /// <summary>
        /// Gets or sets the password history.
        /// </summary>
        /// <value>
        /// The password history.
        /// </value>
        public string PasswordHistory { get; set; }

        /// <summary>
        /// Gets or sets the color of the tag.
        /// </summary>
        /// <value>
        /// The color of the tag.
        /// </value>
        public string TagColor { get; set; }
    }
}

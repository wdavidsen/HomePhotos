using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    /// <summary>
    /// A user DTO with password information.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Web.Dto.User" />
    public class PasswordUser : User
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordUser"/> class.
        /// </summary>
        public PasswordUser() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordUser"/> class using the domain model class.
        /// </summary>
        /// <param name="user">The user.</param>
        public PasswordUser(Model.User user) : base(user) { }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Required]
        [Compare(nameof(PasswordCompare))]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the verification password.
        /// </summary>
        /// <value>
        /// The verification password.
        /// </value>
        [Required]
        [Compare(nameof(Password))]
        public string PasswordCompare { get; set; }
    }
}

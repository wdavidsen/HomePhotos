using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>User registration model.</summary>
    public class RegisterModel
    {
        /// <summary>Gets or sets the name of the user.</summary>
        /// <value>The name of the user.</value>
        [Required]
        public string UserName { get; set; }

        /// <summary>Gets or sets the password.</summary>
        /// <value>The password.</value>
        [Required]
        public string Password { get; set; }

        /// <summary>Gets or sets the password compare.</summary>
        /// <value>The password compare.</value>
        [Compare(nameof(Password))]
        public string PasswordCompare { get; set; }

        /// <summary>Gets or sets the first name.</summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the last name.</summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }

        /// <summary>Gets or sets the email address.</summary>
        /// <value>The email address.</value>
        public string EmailAddress { get; set; }

        /// <summary>Converts to user model.</summary>
        /// <returns>A user model.</returns>
        public Model.User ToUser()
        {
            return new Model.User
            {
                UserName = UserName,
                FirstName = FirstName,
                LastName = LastName,
                EmailAddress = EmailAddress
            };
        }
    }
}

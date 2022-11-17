using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>Change password model.</summary>
    public class ChangePasswordModel
    {
        /// <summary>Gets or sets the name of the user.</summary>
        /// <value>The name of the user.</value>
        [Required]
        public string UserName { get; set; }

        /// <summary>Gets or sets the current password.</summary>
        /// <value>The current password.</value>
        [Required]
        public string CurrentPassword { get; set; }

        /// <summary>Creates new password.</summary>
        /// <value>The new password.</value>
        [Required]
        [Compare(nameof(NewPasswordCompare))]
        public string NewPassword { get; set; }

        /// <summary>Creates new password compare.</summary>
        /// <value>The new password compare.</value>
        [Required]
        public string NewPasswordCompare { get; set; }
    }
}

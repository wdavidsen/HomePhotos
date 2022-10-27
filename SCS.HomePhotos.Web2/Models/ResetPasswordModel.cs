using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>Reset password model.</summary>
    public class ResetPasswordModel
    {
        /// <summary>Gets or sets the name of the user.</summary>
        /// <value>The name of the user.</value>
        [Required]
        public string UserName { get; set; }

        /// <summary>Creates new password.</summary>
        /// <value>The new password.</value>
        [Required]
        [Compare(nameof(NewPasswordCompare))]
        public string NewPassword { get; set; }

        /// <summary>Creates new passwordcompare.</summary>
        /// <value>The new password compare.</value>
        [Required]
        public string NewPasswordCompare { get; set; }
    }
}

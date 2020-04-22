using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    public class PasswordUser : User
    {
        public PasswordUser() { }
        public PasswordUser(Model.User user) : base(user) { }

        [Required]
        [Compare(nameof(PasswordCompare))]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string PasswordCompare { get; set; }
    }
}

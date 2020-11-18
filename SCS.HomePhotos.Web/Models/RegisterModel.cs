using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Models
{
    public class RegisterModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string PasswordCompare { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Model.User ToUser()
        {
            return new Model.User
            {
                UserName = UserName,
                FirstName = FirstName,
                LastName = LastName
            };
        }
    }
}

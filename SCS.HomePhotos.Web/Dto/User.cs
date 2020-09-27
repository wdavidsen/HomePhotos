using SCS.HomePhotos.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    public class User
    {
        public User() { }
        public User(Model.User user)
        {
            UserId = user.UserId;
            Username = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Enabled = user.Enabled;            
            Role = user.Role;
            LastLogin = user.LastLogin;
            FailedLoginCount = user.FailedLoginCount;
            MustChangePassword = user.MustChangePassword;
        }

        public int? UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public bool Enabled { get; set; }

        [Required]
        public RoleType Role { get; set; }


        public DateTime? LastLogin { get; set; }

        public int FailedLoginCount { get; set; }

        public bool MustChangePassword { get; set; }

        public  virtual Model.User ToModel()
        {
            return new Model.User
            {
                UserId = UserId,
                UserName = Username,
                FirstName = FirstName,
                LastName = LastName,
                Enabled = Enabled,
                Role = Role,
                LastLogin = LastLogin,
                FailedLoginCount = FailedLoginCount,
                MustChangePassword = MustChangePassword
            };
        }
    }
}

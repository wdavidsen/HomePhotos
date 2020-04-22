using System;
using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    public class AccountInfo
    {
        public AccountInfo() { }
        public AccountInfo(Model.User user)
        {
            UserId = user.UserId;
            Username = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;            
            Admin = user.Admin;
            LastLogin = user.LastLogin;
        }

        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public bool Admin { get; set; }

        public DateTime? LastLogin { get; set; }

        public  virtual Model.User ToModel()
        {
            return new Model.User
            {
                UserId = UserId,
                UserName = Username,
                FirstName = FirstName,
                LastName = LastName,                
                Admin = Admin,
                LastLogin = LastLogin                
            };
        }
    }
}

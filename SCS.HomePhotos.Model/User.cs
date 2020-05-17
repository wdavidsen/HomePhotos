using Dapper;
using System;

namespace SCS.HomePhotos.Model
{
    public enum RoleType
    {
        Reader = 0,
        Contributer = 1,
        Admin = 2
    }

    [Table("User")]
    public class User
    {
        public User()
        {

        }

        public User(Model.User user)
        {
            UserId = user.UserId;
            UserName = user.UserName;
            PasswordHash = user.PasswordHash;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Enabled = user.Enabled;
            Role = user.Role;
            LastLogin = user.LastLogin;
            FailedLoginCount = user.FailedLoginCount;
            MustChangePassword = user.MustChangePassword;
        }

        [Key]
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool Enabled { get; set; }

        public RoleType Role { get; set; }

        public DateTime? LastLogin { get; set; }

        public int FailedLoginCount { get; set; }

        public bool MustChangePassword { get; set; }

        public string PasswordHistory { get; set; }
    }
}

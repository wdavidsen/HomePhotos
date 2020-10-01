using System.ComponentModel.DataAnnotations;

namespace SCS.HomePhotos.Web.Dto
{
    public class TokenUser : User
    {
        public TokenUser() { }
        public TokenUser(Model.User user, string token = null, string refreshToken = null) : base(user)
        {
            UserId = user.UserId;
            Username = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Token = token;
            RefreshToken = refreshToken;
        }

        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}

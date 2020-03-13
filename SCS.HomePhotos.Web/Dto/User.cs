namespace SCS.HomePhotos.Web.Dto
{
    public class User
    {
        public User() { }
        public User(Model.User user, string token = null, string refreshToken = null)
        {
            Id = user.UserId;
            Username = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Token = token;
            RefreshToken = refreshToken;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public Model.User ToModel()
        {
            return new Model.User
            {
                UserId = Id,
                UserName = Username,
                FirstName = FirstName,
                LastName = LastName
            };
        }
    }
}

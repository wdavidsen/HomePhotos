namespace SCS.HomePhotos.Service
{
    public class RegisterResult
    {
        public RegisterResult()
        {
            UserNameTaken = false;
        }

        public bool Success
        {
            get
            {
                return !(UserNameTaken || PasswordNotStrong);
            }
        }

        public bool UserNameTaken { get; set; }
        public bool PasswordNotStrong { get; set; }
    }
}

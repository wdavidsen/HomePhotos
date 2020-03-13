using SCS.HomePhotos.Model;

namespace SCS.HomePhotos.Service
{
    public class AuthResult
    {
        public virtual bool Success 
        {  
            get
            {
                return !(AttemptsExceeded || MustChangePassword || UserNotExists || UserDisabled || PasswordMismatch);
            }
        }

        public bool AttemptsExceeded { get; set; }
        public bool MustChangePassword { get; set; }
        public bool UserNotExists { get; set; }
        public bool UserDisabled { get; set; }
        public bool PasswordMismatch { get; set; }

        public User User { get; set; }
    }
}

namespace SCS.HomePhotos.Service
{
    public class ChangePasswordResult : AuthResult
    {
        public ChangePasswordResult()
        {
        }

        public bool PasswordUsedPreviously { get; set; }

        public bool PasswordNotStrong { get; set; }

        public override bool Success => base.Success && !PasswordUsedPreviously && !PasswordNotStrong;

        public ChangePasswordResult(AuthResult authResult)
        {
            AttemptsExceeded = authResult.AttemptsExceeded;
            MustChangePassword = authResult.MustChangePassword;
            UserNotExists = authResult.UserNotExists;
            UserDisabled = authResult.UserDisabled;
            PasswordMismatch = authResult.PasswordMismatch;
        }
    }
}

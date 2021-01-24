namespace SCS.HomePhotos.Service
{
    public class ChangePasswordResult : AuthResult
    {
        public ChangePasswordResult()
        {
        }

        public bool PasswordUsedPreviously { get; set; }

        public bool PasswordNotStrong { get; internal set; }

        public override bool Success => base.Success && !PasswordUsedPreviously;

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

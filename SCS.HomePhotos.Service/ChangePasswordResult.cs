namespace SCS.HomePhotos.Service
{
    /// <summary>
    /// The change password result.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.Service.AuthResult" />
    public sealed class ChangePasswordResult : AuthResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordResult"/> class.
        /// </summary>
        public ChangePasswordResult()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether password was used previously.
        /// </summary>
        /// <value>
        ///   <c>true</c> if password was used previously; otherwise, <c>false</c>.
        /// </value>
        public bool PasswordUsedPreviously { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether password is not strong.
        /// </summary>
        /// <value>
        ///   <c>true</c> if password is not strong; otherwise, <c>false</c>.
        /// </value>
        public bool PasswordNotStrong { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="AuthResult" /> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public override bool Success => base.Success && !PasswordUsedPreviously && !PasswordNotStrong;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordResult"/> class.
        /// </summary>
        /// <param name="authResult">The authentication result.</param>
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

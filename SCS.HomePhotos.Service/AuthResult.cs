using SCS.HomePhotos.Model;

namespace SCS.HomePhotos.Service
{
    /// <summary>
    /// An authentication result.
    /// </summary>
    public class AuthResult
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="AuthResult"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Success 
        {  
            get
            {
                return !(AttemptsExceeded || MustChangePassword || UserNotExists || UserDisabled || PasswordMismatch);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether attempts were exceeded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if attempts were exceeded; otherwise, <c>false</c>.
        /// </value>
        public bool AttemptsExceeded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user must change password.
        /// </summary>
        /// <value>
        ///   <c>true</c> if user must change password; otherwise, <c>false</c>.
        /// </value>
        public bool MustChangePassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user does not exists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if user does not exists; otherwise, <c>false</c>.
        /// </value>
        public bool UserNotExists { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user is disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if user is disabled; otherwise, <c>false</c>.
        /// </value>
        public bool UserDisabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether password does not mismatch.
        /// </summary>
        /// <value>
        ///   <c>true</c> if password does not mismatch; otherwise, <c>false</c>.
        /// </value>
        public bool PasswordMismatch { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public User User { get; set; }        
    }
}

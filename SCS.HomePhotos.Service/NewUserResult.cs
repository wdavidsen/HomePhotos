namespace SCS.HomePhotos.Service
{
    /// <summary>
    /// Result of a user registering with app.
    /// </summary>
    public class RegisterResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterResult"/> class.
        /// </summary>
        public RegisterResult()
        {
            UserNameTaken = false;
            PasswordNotStrong = false;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="RegisterResult"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success
        {
            get
            {
                return !(UserNameTaken || PasswordNotStrong);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether username is taken.
        /// </summary>
        /// <value>
        ///   <c>true</c> if username is taken; otherwise, <c>false</c>.
        /// </value>
        public bool UserNameTaken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether password is not strong enough.
        /// </summary>
        /// <value>
        ///   <c>true</c> if password is not strong; otherwise, <c>false</c>.
        /// </value>
        public bool PasswordNotStrong { get; set; }
    }
}

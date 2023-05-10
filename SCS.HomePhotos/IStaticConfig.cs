namespace SCS.HomePhotos
{
    /// <summary>
    /// The application's static configuration set via appsettings.json file.
    /// </summary>
    public interface IStaticConfig
    {
        /// <summary>
        /// Gets or sets the database path.
        /// </summary>
        /// <value>
        /// The database path.
        /// </value>
        string DatabasePath { get; set; }

        /// <summary>
        /// Gets or sets the token expiration minutes.
        /// </summary>
        /// <value>
        /// The token expiration minutes.
        /// </value>
        int TokenExpirationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the refresh token expiration days.
        /// </summary>
        /// <value>
        /// The refresh token expiration days.
        /// </value>
        int RefreshTokenExpirationDays { get; set; }

        /// <summary>
        /// Gets or sets the maximum failed logins.
        /// </summary>
        /// <value>
        /// The maximum failed logins.
        /// </value>
        int MaxFailedLogins { get; set; }

        /// <summary>
        /// Gets or sets the image index parallelism.
        /// </summary>
        /// <value>
        /// The image index parallelism.
        /// </value>
        int ImageIndexParallelism { get; set; }

        /// <summary>
        /// Gets or sets the maximum image file size bytes.
        /// </summary>
        /// <value>
        /// The maximum image file size bytes.
        /// </value>
        int MaxImageFileSizeBytes { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed index directory failures.
        /// </summary>
        /// <value>
        /// The maximum allowed index directory failures.
        /// </value>
        int MaxAllowedIndexDirectoryFailures { get; set; }

        /// <summary>
        /// Gets or sets the image resize quality.
        /// </summary>
        /// <value>
        /// The image resize quality.
        /// </value>
        int ImageResizeQuality { get; set; }

        /// <summary>
        /// Gets or sets the log retention days.
        /// </summary>
        /// <value>
        /// The log retention days.
        /// </value>
        int LogRetentionDays { get; set; }

        /// <summary>
        /// Gets or sets the password requirements.
        /// </summary>
        /// <value>
        /// The password requirements.
        /// </value>
        PasswordRequirements PasswordRequirements { get; set; }

        /// <summary>
        /// Gets or sets the image URL encryption key.
        /// </summary>
        /// <value>
        /// The URL encryption key.
        /// </value>
        public string ImageEncryptKey { get; set; }

        /// <summary>
        /// Gets or sets the image URL encryption passcode.
        /// </summary>
        /// <value>
        /// The URL encryption passcode.
        /// </value>
        public string ImageEncryptPasscode { get; set; }

        /// <summary>
        /// Gets or sets the photo expiration days.
        /// </summary>
        /// <value>
        /// The photo expiration days.
        /// </value>
        int PhotoExpirationDays { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to automatic approve new user registrations.
        /// </summary>
        /// <value>
        ///   <c>true</c> if automatically approving new user registrations; otherwise, <c>false</c>.
        /// </value>
        bool AutoApproveRegistrations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to make the first registrated user an Admin. Set this to false after setup for increased security.
        /// </summary>
        /// <value>
        ///   <c>true</c> to make first registrated user an Admin; otherwise, <c>false</c>.
        /// </value>
        bool MakeFirstRegistrationAdmin { get; set; }
    }
}
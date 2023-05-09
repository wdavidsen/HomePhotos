namespace SCS.HomePhotos.Web.Models
{
    /// <summary>
    /// Next step information for after a user successfully registers.
    /// </summary>
    public class RegisteredInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether user was auto-approved.
        /// </summary>
        /// <value>
        ///   <c>true</c> if auto-approved; otherwise, <c>false</c>.
        /// </value>
        public bool AutoApproved { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredInfo"/> class.
        /// </summary>
        /// <param name="autoApproved">if set to <c>true</c> [automatic approved].</param>
        public RegisteredInfo(bool autoApproved)
        {
            AutoApproved = autoApproved;
        }
    }
}

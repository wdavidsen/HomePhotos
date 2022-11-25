namespace SCS.HomePhotos.Web.Middleware
{
    /// <summary>
    /// The global middleware error handler.
    /// </summary>
    internal class GlobalErrorDetails
    {
        /// <summary>Initializes a new instance of the <see cref="GlobalErrorDetails"/> class.</summary>
        public GlobalErrorDetails()
        {
        }

        /// <summary>Gets or sets the status code.</summary>
        /// <value>The status code.</value>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Converts to a JSON error string for output response.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{{\"message\": \"{StatusCode} {Message.Replace(@"""", @"""""", StringComparison.CurrentCultureIgnoreCase)}\"}}";
        }
    }
}
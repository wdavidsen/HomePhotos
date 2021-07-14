namespace SCS.HomePhotos
{
    /// <summary>
    /// Wrapper for all password requirements.
    /// </summary>
    public sealed class PasswordRequirements
    {
        /// <summary>
        /// Gets or sets the minimum password length.
        /// </summary>
        /// <value>
        /// The minimum password length.
        /// </value>
        public int MinLength { get; set; }

        /// <summary>
        /// Gets or sets the required number of uppercase characters.
        /// </summary>
        /// <value>
        /// The required number of uppercase characters.
        /// </value>
        public int UppercaseCharacters { get; set; }

        /// <summary>
        /// Gets or sets the required number of digits.
        /// </summary>
        /// <value>
        /// The required number of digits.
        /// </value>
        public int Digits { get; set; }

        /// <summary>
        /// Gets or sets the required number of special characters.
        /// </summary>
        /// <value>
        /// The required number of special characters.
        /// </value>
        public int SpecialCharacters { get; set; }
    }
}

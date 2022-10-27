namespace SCS.HomePhotos.Web.Models
{
    /// <summary>An ambiguous tag state.</summary>
    public class AmbiguousTagState : TagState
    {
        /// <summary>Initializes a new instance of the <see cref="AmbiguousTagState" /> class.</summary>
        public AmbiguousTagState() : base()
        {
            Indeterminate = false;
            AllowIndeterminate = false;
        }

        /// <summary>Gets or sets a value indicating whether this <see cref="AmbiguousTagState" /> is indeterminate.</summary>
        /// <value>
        ///   <c>true</c> if indeterminate; otherwise, <c>false</c>.</value>
        public bool Indeterminate { get; set; }

        /// <summary>Gets or sets a value indicating whether to allow indeterminate.</summary>
        /// <value>
        ///   <c>true</c> if allowing indeterminate; otherwise, <c>false</c>.</value>
        public bool AllowIndeterminate { get; set; }
    }
}
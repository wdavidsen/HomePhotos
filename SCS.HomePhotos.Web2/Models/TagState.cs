namespace SCS.HomePhotos.Web.Models
{
    /// <summary>Tag state model.</summary>
    public class TagState
    {
        /// <summary>Initializes a new instance of the <see cref="TagState" /> class.</summary>
        public TagState()
        {
            Id = 0;
            TagName = "";
            Checked = false;
        }
        /// <summary>Initializes a new instance of the <see cref="TagState" /> class.</summary>
        /// <param name="tagId">The tag identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="isChecked">if set to <c>true</c> is checked.</param>
        public TagState(int tagId, string name, bool isChecked) : this()
        {
            Id = tagId;
            TagName = name;
            Checked = isChecked;
        }

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>Gets or sets the name of the tag.</summary>
        /// <value>The name of the tag.</value>
        public string TagName { get; set; }

        /// <summary>Gets or sets a value indicating whether this <see cref="TagState" /> is checked.</summary>
        /// <value>
        ///   <c>true</c> if checked; otherwise, <c>false</c>.</value>
        public bool Checked { get; set; }
    }
}

namespace SCS.HomePhotos.Web.Models
{
    public class AmbiguousTagState : TagState
    {
        public AmbiguousTagState() : base()
        {
            Indeterminate = false;
            AllowIndeterminate = false;
        }

        public bool Indeterminate { get; set; }

        public bool AllowIndeterminate { get; set; }
    }
}
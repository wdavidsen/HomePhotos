namespace SCS.HomePhotos.Web.Models
{
    public class AmbiguousTagState : TagState
    {
        public AmbiguousTagState() : base()
        {
            AppliesToAll = true;
        }
        public AmbiguousTagState(int tagId, string name, bool isChecked, bool appliesToAll) : base(tagId, name, isChecked)
        {
            AppliesToAll = appliesToAll;
        }

        public bool AppliesToAll { get; set; }
    }
}
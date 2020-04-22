namespace SCS.HomePhotos.Web.Models
{
    public class TagState
    {
        public TagState()
        {
            Id = 0;
            TagName = "";
            Checked = false;
        }
        public TagState(int tagId, string name, bool isChecked) : this()
        {
            Id = tagId;
            TagName = name;
            Checked = isChecked;
        }

        public int Id { get; set; }
        public string TagName { get; set; }
        public bool Checked { get; set; }
    }
}

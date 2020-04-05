namespace SCS.HomePhotos.Web.Models
{
    public class TagState
    {
        public TagState()
        {
            Id = 0;
            Name = "";
            Checked = false;
        }
        public TagState(int tagId, string name, bool isChecked) : this()
        {
            Id = tagId;
            Name = name;
            Checked = isChecked;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }
    }
}

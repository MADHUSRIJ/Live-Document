namespace Live_Document___Rich_Text_Editor.Models
{
    public class DocumentEntityModel
    {
        public int DocumentId { get; set; }
        public string DocumentTitle { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get;  }
        public DateTime LastEdited { get; set; }
    }
}

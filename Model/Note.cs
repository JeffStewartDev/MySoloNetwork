
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSN.Model
{
    public class Note
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public bool Visible { get; set; } = true;
        [NotMapped]
        public string NoteText
        {
            get
            {
                return
            System.Text.Encodings.Web.HtmlEncoder.Default.Encode(Text)
            .Replace("&#xA;", "<br/>") ;
            }
        }
    }
}
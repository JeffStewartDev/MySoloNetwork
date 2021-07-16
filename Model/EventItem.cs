using System;
using System.Collections.Generic;
using System.Text;

namespace MSN.Model
{
    public class EventItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime DateStarts { get; set; } = DateTime.Now;
        public DateTime DateEnds { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public bool Visible { get; set; } = true;
        public Guid? ImagePostId { get; set; }
        public ImagePost ImagePost { get; set; }
    }
}


using System;
using System.Collections.Generic;

namespace MSN.Model
{
    public class Album
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }=string.Empty;
        public string Location { get; set; }=string.Empty;
        public string Comment { get; set; }=string.Empty;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public bool Visible { get; set; } = true;
        public Guid MainImageId { get; set; }
        public ICollection<ImagePost> ImagePosts { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace MSN.Model
{
    public class UpdatePostItem
    {
        public Guid Id { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime DateCreated { get; set; }
        public PostTypes PostType { get; set; }

    }
}

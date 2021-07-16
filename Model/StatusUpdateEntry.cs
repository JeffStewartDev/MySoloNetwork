
using System;

namespace MSN.Model
{
    public class StatusUpdateEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Text { get; set; }=string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public bool Visible { get; set; } = true;
    }
}
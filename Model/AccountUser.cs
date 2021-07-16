
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSN.Model
{
    public class AccountUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NameFirst { get; set; } = string.Empty;
        public string NameLast { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public DateTime DateLastLogin { get; set; }
        public bool Visible { get; set; } = true;
        [NotMapped]
        public string Name
        {
            get
            {
                return
            System.Text.Encodings.Web.HtmlEncoder.Default.Encode($"{NameFirst} {NameLast}")
            .Replace("&#xA;", "<br/>") ;
            }
        }
    }
}
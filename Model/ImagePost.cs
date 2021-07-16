
using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MSN.Model
{
    public class ImagePost
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Nullable<Guid> AlbumId { get; set; } = null;
        public string Entry { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public bool Visible { get; set; } = true;
        public string Base64Image { get; set; }
        public string ImageFileLocation { get; set; }


     }
}
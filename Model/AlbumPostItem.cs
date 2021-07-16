using System;
using System.Collections.Generic;
using System.Text;

namespace MSN.Model
{
    public class AlbumPostItem:UpdatePostItem
    {
        public new PostTypes PostType { get; set; } = PostTypes.Album;
        public ImagePost MainPhoto { get; set; }

    }
}

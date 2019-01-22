using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GAPINewMediaItems : GAPIBaseObject
    {
        public string albumId { get; set; }
        public List<GLAPINewMediaItem> newMediaItems { get; set; } = new List<GLAPINewMediaItem>();
        public GAPIAlbumPosition albumPosition { get; set; } = new GAPIAlbumPosition();
    }
}

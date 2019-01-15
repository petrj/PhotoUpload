using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GAPIShareInfo : GAPIBaseObject
    {
        GAPISharedAlbumOptions sharedAlbumOptions { get; set; } = new GAPISharedAlbumOptions();

        public string shareableUrl { get; set; }
        public string shareToken { get; set; }
        public bool isJoined { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GAPIAlbumPosition : GAPIBaseObject
    {
        public GAPIPositionType position { get; set; } = GAPIPositionType.LAST_IN_ALBUM;
        public string relativeMediaItemId { get; set; }
        public string relativeEnrichmentItemId { get; set; }
    }
}

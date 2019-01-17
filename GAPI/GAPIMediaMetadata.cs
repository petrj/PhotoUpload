using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GAPIMediaMetadata : GAPIBaseObject
    {
        public string creationTime { get; set; }
        public string width { get; set; }
        public string height { get; set; }

        // Union field metadata can be only one of the following:
        public GAPIPhoto photo { get; set; } = new GAPIPhoto();
        public GAPIVideo video { get; set; } = new GAPIVideo();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GAPIMediaItem : GAPIBaseObject
    {
        public string id { get; set; }
        public string description { get; set; }
        public string productUrl { get; set; }
        public string baseUrl { get; set; }
        public string mimeType { get; set; }

        public GAPIMediaMetadata mediaMetadata { get; set; } = new GAPIMediaMetadata();
        public GAPIContributorInfo contributorInfo { get; set; } = new GAPIContributorInfo();

        public string filename { get; set; }
    }
}

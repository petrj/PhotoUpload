using System;

namespace GAPI
{
    public partial class GAPIAbout : GAPIBaseObject
    {
        // https://developers.google.com/drive/api/v2/reference/about/get

        public string kind { get; set; }
        public string etag { get; set; }

        public string selfLink { get; set; }
        public string name { get; set; }
      
        public long quotaBytesTotal { get; set; }
        public long quotaBytesUsed { get; set; }
        public long quotaBytesUsedAggregate { get; set; }
        public long quotaBytesUsedInTrash { get; set; }
        public string quotaType { get; set; }
    }
}

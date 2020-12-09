using System;
using System.Collections.Generic;

namespace GAPI
{
    public partial class GAPIItemsList
    {
        public List<GAPIMediaItem> mediaItems { get; set; } = new List<GAPIMediaItem>();
        public string nextPageToken { get; set; }
    }
}

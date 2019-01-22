using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

namespace GAPI
{
    public partial class GAPIAlbum : GAPIBaseObject
    {
        public string id { get; set; }
        public string title { get; set; }
        public string productUrl { get; set; }
        public bool isWriteable { get; set; }

        public GAPIShareInfo shareInfo { get; set; } = new GAPIShareInfo();

        public string mediaItemsCount { get; set; }
        public string coverPhotoBaseUrl { get; set; }
        public string coverPhotoMediaItemId { get; set; }

    }
}

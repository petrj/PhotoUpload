using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

namespace GAPI
{
    public partial class GAPIAlbumsList : GAPIBaseObject
    {
        public List<GAPIAlbum> albums { get; set; } = new List<GAPIAlbum>();
        public string nextPageToken { get; set; }
    }
}

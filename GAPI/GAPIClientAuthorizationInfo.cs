using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

namespace GAPI
{
    public class GAPIClientAuthorizationInfo : GAPIBaseObject
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }

        public List<string> scopes { get; set; }
    }
}

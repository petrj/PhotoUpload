using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

namespace GAPI
{
    public partial class GAPIAccessToken : GAPIBaseObject
    {
        // fill after JsonConvert.DeserializeObject
        public DateTime expires_at { get; set; }

        public string refresh_token_urlencoded
        {
            get
            {
                return WebUtility.UrlEncode(refresh_token);
            }
        }

        public GAPIAccessToken()
        {
            expires_at = DateTime.MinValue;
        }
    }
}

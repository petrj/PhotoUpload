using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

namespace GAPI
{
    public class GAPIAccessToken : GAPIBaseObject
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }

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

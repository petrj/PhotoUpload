using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

namespace GAPI
{
    public class GAPIAlbumsList : GAPIBaseObject
    {
        public List<GAPIAlbum> albums { get; set; }

        public static GAPIAlbumsList GetAlbums(GAPIAccessToken token, int pageSize = 20, string pageToken = null)
        {
            // https://developers.google.com/photos/library/reference/rest/v1/albums/list

            try
            {
                var url = "https://photoslibrary.googleapis.com/v1/albums";

                var responseString = GAPIAccountConnection.SendRequest(url, null, "GET", token.access_token);

                return JsonConvert.DeserializeObject<GAPIAlbumsList>(responseString);
            }

            catch (WebException ex)
            {
                Console.WriteLine(ex);

                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        Console.WriteLine($"Status code: {(int)response.StatusCode}");
                    }
                }
                throw;
            }
        }
    }
}

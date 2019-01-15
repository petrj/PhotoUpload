using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

namespace GAPI
{
    public class GAPIAlbum : GAPIBaseObject
    {
        public string id { get; set; }
        public string title { get; set; }
        public string productUrl { get; set; }
        public bool isWriteable { get; set; }

        GAPIShareInfo shareInfo { get; set; } = new GAPIShareInfo();

        public string mediaItemsCount { get; set; }
        public string coverPhotoBaseUrl { get; set; }
        public string coverPhotoMediaItemId { get; set; }

        public static GAPIAlbum CreateAlbum(GAPIAccessToken token, string albumTitle)
        {
            // https://developers.google.com/photos/library/reference/rest/v1/albums/create

            try
            {
                var url = $"https://photoslibrary.googleapis.com/v1/albums";

                var newAlbum = new GAPINewAlbum();
                newAlbum.album.title = albumTitle;

                var postData = JsonConvert.SerializeObject(newAlbum);

                newAlbum.SaveToFile("newAlbum.json");

                var responseString = GAPIAccountConnection.SendRequest(url, postData, "POST", token.access_token, "application/json");

                var album = JsonConvert.DeserializeObject<GAPIAlbum>(responseString);

                return album;
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

        public static GAPIAlbum GetAlbum(GAPIAccessToken token, string id)
        {
            // https://developers.google.com/photos/library/reference/rest/v1/albums/list

            var idFix = WebUtility.UrlEncode(id);
            try
            {
                var url = $"https://photoslibrary.googleapis.com/v1/albums/{idFix}";

                var responseString = GAPIAccountConnection.SendRequest(url, null, "GET", token.access_token);

                var alb = JsonConvert.DeserializeObject<GAPIAlbum>(responseString);

                Console.WriteLine(alb.ToString());

                return alb;
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

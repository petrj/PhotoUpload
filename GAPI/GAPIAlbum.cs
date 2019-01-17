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

                return GAPIAccountConnection.SendRequest<GAPIAlbum>(url, newAlbum, token.access_token);
            }
            catch (WebException ex)
            {
                Logger.WriteToLog(ex);
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

                var alb = GAPIAccountConnection.SendRequest<GAPIAlbum>(url, null, "GET", token.access_token);

                Logger.WriteToLog(alb.ToString());

                return alb;
            }
            catch (WebException ex)
            {
                Logger.WriteToLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Adding media to album
        /// </summary>
        /// <param name="items">
        /// Key .... upload_token (result from UploadFile)
        /// Value .. item description
        /// </param>
        /// <returns></returns>
        public static GAPINewMediaItemResults AddMediaItemsToAlbum(GAPIAccessToken token, string albumId, Dictionary<string,string> items)
        {
            // https://developers.google.com/photos/library/reference/rest/v1/mediaItems/batchCreate

            var url = "https://photoslibrary.googleapis.com/v1/mediaItems:batchCreate";

            var newMediaItems = new GAPINewMediaItems();
            newMediaItems.albumId = albumId;

            foreach (var kvp in items)
            {
                var newMediaItem = new GLAPINewMediaItem();
                newMediaItem.description = kvp.Value;
                newMediaItem.simpleMediaItem.uploadToken = kvp.Key;

                newMediaItems.newMediaItems.Add(newMediaItem);
            }

            try
            {
                var newMediaItemResults = GAPIAccountConnection.SendRequest<GAPINewMediaItemResults>(url, newMediaItems, token.access_token);

                Logger.WriteToLog(newMediaItemResults.ToString());

                return newMediaItemResults;
            }
            catch (WebException ex)
            {
                Logger.WriteToLog(ex);
                throw;
            }
        }
    }
}

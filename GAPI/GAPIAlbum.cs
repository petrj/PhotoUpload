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

        public GAPIShareInfo shareInfo { get; set; } = new GAPIShareInfo();

        public string mediaItemsCount { get; set; }
        public string coverPhotoBaseUrl { get; set; }
        public string coverPhotoMediaItemId { get; set; }

        public static GAPIAlbum CreateAlbum(GAPIAccountConnection conn, string albumTitle)
        {
            Logger.Info($"Creating album {albumTitle}");

            // https://developers.google.com/photos/library/reference/rest/v1/albums/create

            try
            {
                var url = $"https://photoslibrary.googleapis.com/v1/albums";

                var newAlbum = new GAPINewAlbum();
                newAlbum.album.title = albumTitle;

                var postData = JsonConvert.SerializeObject(newAlbum);

                newAlbum.SaveToFile("newAlbum.json");

                return GAPIAccountConnection.SendRequest<GAPIAlbum>(url, newAlbum, conn);
            }
            catch (WebException ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static GAPIAlbum GetAlbum(GAPIAccountConnection conn, string id)
        {
            Logger.Info($"Geting album id {id}");

            // https://developers.google.com/photos/library/reference/rest/v1/albums/list

            var idFix = WebUtility.UrlEncode(id);
            try
            {
                var url = $"https://photoslibrary.googleapis.com/v1/albums/{idFix}";

                var alb = GAPIAccountConnection.SendRequest<GAPIAlbum>(url, null, "GET", conn);

                Logger.Info(alb.ToString());

                return alb;
            }
            catch (WebException ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static GAPINewMediaItemResults AddMediaItemToAlbum(GAPIAccountConnection conn, string albumId, string fileName)
        {
            Logger.Info($"Addding media item (file {fileName}) to album id {albumId}");

            try
            {
                var fileToken = conn.UploadFile(fileName);

                var mediaItems = new Dictionary<string, string>() { { fileToken, fileName } };

                return AddMediaItemsToAlbum(conn, albumId, mediaItems);
            }
            catch (WebException ex)
            {
                Logger.Error(ex);
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
        public static GAPINewMediaItemResults AddMediaItemsToAlbum(GAPIAccountConnection conn, string albumId, Dictionary<string,string> items)
        {
            Logger.Info($"Addding media items to album id {albumId}");

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
                var newMediaItemResults = GAPIAccountConnection.SendRequest<GAPINewMediaItemResults>(url, newMediaItems, conn);

                Logger.Info(newMediaItemResults.ToString());

                return newMediaItemResults;
            }
            catch (WebException ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}

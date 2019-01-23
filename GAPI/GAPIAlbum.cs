using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

namespace GAPI
{
    public partial class GAPIAlbum : GAPIBaseObject
    {
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

                return GAPICommunication.SendRequest<GAPIAlbum>(url, newAlbum, conn);
            }
            catch (Exception ex)
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

                var alb = GAPICommunication.SendRequest<GAPIAlbum>(url, null, "GET", conn);

                return alb;
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
            try
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

                var newMediaItemResults = GAPICommunication.SendRequest<GAPINewMediaItemResults>(url, newMediaItems, conn);

                return newMediaItemResults;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}

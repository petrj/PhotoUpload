using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

namespace GAPI
{
    public class GAPIAlbumsList : GAPIBaseObject
    {
        public List<GAPIAlbum> albums { get; set; } = new List<GAPIAlbum>();
        public string nextPageToken { get; set; }

        public static GAPIAlbumsList GetAllAlbums(GAPIAccountConnection conn)
        {
            try
            {
                var result = new GAPIAlbumsList();

                string pageToken = null;

                do
                {
                    var albums = GetAlbums(conn, 50, pageToken);

                    result.albums.AddRange(albums.albums);

                    pageToken = albums.nextPageToken;

                } while (pageToken != null);

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static GAPIAlbumsList GetAlbums(GAPIAccountConnection conn, int pageSize = 20, string pageToken = null)
        {
            // https://developers.google.com/photos/library/reference/rest/v1/albums/list

            try
            {
                var url = "https://photoslibrary.googleapis.com/v1/albums";

                url += $"?pageSize={pageSize.ToString()}";

                if (!String.IsNullOrEmpty(pageToken))
                {
                    url += $"&pageToken={pageToken}";
                }

                return GAPIAccountConnection.SendRequest<GAPIAlbumsList>(url, null , "GET", conn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}

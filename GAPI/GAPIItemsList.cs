using System;
namespace GAPI
{
    public partial class GAPIItemsList : GAPIBaseObject
    {
        public static GAPIItemsList GetAllItems(GAPIAccountConnection conn)
        {
            try
            {
                var result = new GAPIItemsList();

                string pageToken = null;

                do
                {
                    var items = GetItems(conn, 50, pageToken);

                    result.mediaItems.AddRange(items.mediaItems);

                    pageToken = items.nextPageToken;

                } while (pageToken != null);

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        private static GAPIItemsList GetItems(GAPIAccountConnection conn, int pageSize = 20, string pageToken = null)
        {
            // https://developers.google.com/photos/library/reference/rest/v1/mediaItems/list

            try
            {
                var url = "https://photoslibrary.googleapis.com/v1/mediaItems";

                url += $"?pageSize={pageSize.ToString()}";

                if (!String.IsNullOrEmpty(pageToken))
                {
                    url += $"&pageToken={pageToken}";
                }

                return GAPICommunication.SendRequest<GAPIItemsList>(url, null, "GET", conn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}

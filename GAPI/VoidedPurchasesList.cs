using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class VoidedPurchasesList : GAPIBaseObject
    {


        public static VoidedPurchasesList GetVoidedPurchases(GAPIAccountConnection conn, string packageName, int pageSize = 20, string pageToken = null)
        {
            // https://developers.google.com/android-publisher/voided-purchases?authuser=0

            try
            {
                var url = $"https://www.googleapis.com/androidpublisher/v3/applications/{packageName}/purchases/voidedpurchases?access_token={conn.AccessToken.access_token}";

                url += $"?pageSize={pageSize.ToString()}";

                if (!String.IsNullOrEmpty(pageToken))
                {
                    url += $"&pageToken={pageToken}";
                }

                return GAPICommunication.SendRequest<VoidedPurchasesList>(url, null, "GET", conn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}

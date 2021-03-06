﻿using System;

namespace GAPI
{
    public partial class GAPIAbout
    {
        // https://developers.google.com/drive/api/v2/reference/about/get

        public string GBytesTotal
        {
            get
            {
                return (quotaBytesTotal/Math.Pow(10,9)).ToString("N2");
            }
        }

        public string GBytesDriveTotal
        {
            get
            {
                return (quotaBytesUsed / Math.Pow(10, 9)).ToString("N2");
            }
        }

        public string GBytesTrashTotal
        {
            get
            {
                return (quotaBytesUsedInTrash / Math.Pow(10, 9)).ToString("N2");
            }
        }

        public string GBytesUsedByAppsTotal
        {
            get
            {
                return (quotaBytesUsedAggregate / Math.Pow(10, 9)).ToString("N2");
            }
        }

        public string GBytesFreeTotal
        {
            get
            {
                var freeBytes = (quotaBytesTotal - quotaBytesUsedAggregate);
                if (freeBytes == 0)
                    return 0.ToString("N2");

                return (freeBytes / Math.Pow(10, 9)).ToString("N2");
            }
        }

        public static GAPIAbout AboutUser(GAPIAccountConnection conn)
        {
            try
            {
                var url = "https://www.googleapis.com/drive/v2/about";

                var about = GAPICommunication.SendRequest<GAPIAbout>(url, null, "GET", conn);
               
                return about;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}

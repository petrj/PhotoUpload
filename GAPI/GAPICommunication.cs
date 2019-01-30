using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using GAPI;
using System.IO;
using System.Text;
using System.Globalization;


namespace GAPI
{
    public class GAPICommunication
    {
        /// <summary>
        /// Sends the request.
        /// </summary>
        /// <returns>Response string</returns>
        /// <param name="request">Request.</param>
        /// <param name="ASCIIPOSTdata">POST Data</param>
        /// <param name="conn">Initialized GAPIAccountConnection</param>
        private static string SendRequestBase(HttpWebRequest request,
           string ASCIIPOSTdata,
           GAPIAccountConnection conn = null)
        {
            try
            {

                if (request.Method == "GET")
                {
                    Logger.Debug($"Sending GET request to url: {request.RequestUri}");
                }
                else
                {
                    Logger.Debug($"Sending POST request to url: {request.RequestUri}");

                    if (request.ContentLength >= 0)
                    {
                        Logger.Debug($"Request ContentLength: {request.ContentLength}");
                    }

                    if (ASCIIPOSTdata != null)
                    {
                        Logger.Debug($"Posting data length: {ASCIIPOSTdata.Length}");

                        if (request.ContentType != "application/octet-stream")
                        {
                            Logger.Debug($"Posting data: {ASCIIPOSTdata}");
                        }
                    }
                }

                Logger.Debug($"ContentType: {request.ContentType}");

                if (conn != null)
                {
                    if (conn.AccessToken.expires_at < DateTime.Now)
                    {
                        Logger.Info($"Access token expired at [{conn.AccessToken.expires_at.ToString()}]");
                        conn.RefreshAccessToken();
                    }

                    var accessToken = WebUtility.UrlEncode(conn.AccessToken.access_token);
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    request.PreAuthenticate = true;
                }

                if (request.Method == "POST" && ASCIIPOSTdata != null)
                {
                    // adding post data

                    var postData = string.IsNullOrEmpty(ASCIIPOSTdata)
                    ? new byte[0]
                    : Encoding.ASCII.GetBytes(ASCIIPOSTdata);

                    request.ContentLength = ASCIIPOSTdata.Length;

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(postData, 0, ASCIIPOSTdata.Length);
                    }
                }

                request.Timeout = 10 * 60 * 1000; // 10 min timeout

                Logger.Debug($"Method: {request.Method}");
                Logger.Debug($"RequestUri: {request.RequestUri}");
                Logger.Debug($"Timeout: {request.Timeout}");
                Logger.Debug($"ContentType: {request.ContentType}");
                if (request.ContentLength > 0)
                {
                    Logger.Debug($"ContentLength: {request.ContentLength}");
                }

                foreach (var header in request.Headers)
                {
                    Logger.Debug($"Header: {header.ToString()}");
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    string responseString;
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        responseString = sr.ReadToEnd();
                    }

                    Logger.Debug($"Response: {responseString}");
                    Logger.Debug($"StatusCode: {response.StatusCode}");
                    Logger.Debug($"StatusDescription: {response.StatusDescription}");

                    Logger.Debug($"ContentLength: {response.ContentLength}");
                    Logger.Debug($"ContentType: {response.ContentType}");
                    Logger.Debug($"ContentEncoding: {response.ContentEncoding}");

                    return responseString;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static T SendRequest<T>(string url,
                GAPIBaseObject obj,
                GAPIAccountConnection conn)
        {
            return SendRequest<T>(url, JsonConvert.SerializeObject(obj), "POST", conn, "application/json");
        }

        public static string SendRequest(HttpWebRequest request,
               GAPIAccountConnection conn = null)
        {
            return SendRequestBase(request, null, conn);
        }

        public static T SendRequest<T>(string url,
                string data,
                string method,
                GAPIAccountConnection conn = null,
                string contentType = "application/x-www-form-urlencoded")
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = method;
                request.ContentType = contentType;
                request.Accept = "application/json";

                var responseString = SendRequestBase(request, data, conn);

                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        // https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        public static string NormalizeStringForUrl(string name)
        {
            String normalizedString = name.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                switch (CharUnicodeInfo.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        stringBuilder.Append(c);
                        break;
                    case UnicodeCategory.SpaceSeparator:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.DashPunctuation:
                        stringBuilder.Append('_');
                        break;
                }
            }
            string result = stringBuilder.ToString();
            return String.Join("_", result.Split(new char[] { '_' }
                , StringSplitOptions.RemoveEmptyEntries)); // remove duplicate underscores
        }
    }
}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using GAPI;
using System.IO;
using System.Text;

namespace GAPI
{
    public class GAPIAccountConnection
    {
        public GAPIClientAuthorizationInfo AuthInfo { get; set; }
        public GAPIAccessToken AccessToken { get; set; }

        public GAPIAccountConnection(GAPIClientAuthorizationInfo authInfo)
        {
            AuthInfo = authInfo;
        }

        public void Connect()
        {
            var tokenPath = $"{GAPIBaseObject.AppDataDir}token.json";

            if (File.Exists(tokenPath))
            {
                AccessToken = GAPIBaseObject.LoadFromFile<GAPIAccessToken>("token.json");

                if (string.IsNullOrEmpty(AccessToken.refresh_token))
                {
                    Authenticate();
                }
                else
                {
                    if (AccessToken.expires_at < DateTime.Now)
                    {
                        RefreshAccessToken();
                        AccessToken.SaveToFile("token.json");
                    }
                }
            }
            else
            {
                Authenticate();
            }
        }

        public void Authenticate()
        {
            // authenticate user & user consent

            var browserUrl = GetUrlForAuthCode();

            Console.WriteLine("Authorize on url:");
            Console.WriteLine();
            Console.WriteLine(browserUrl);
            Console.WriteLine();

            Console.Write("Paste auth code:");
            var code = Console.ReadLine();

            ReceiveAccessToken(code);

            AccessToken.SaveToFile("token.json");
        }

        public string GetUrlForAuthCode()
        {
            // https://developers.google.com/identity/protocols/OAuth2InstalledApp

            var redirectURI = "urn:ietf:wg:oauth:2.0:oob";
            string scope = String.Join(" ", AuthInfo.scopes.ToArray());
            scope = WebUtility.UrlEncode(scope);

            var url = "https://accounts.google.com/o/oauth2/v2/auth";

            string postData = $"client_id={AuthInfo.client_id}";
            postData += $"&redirect_uri={redirectURI}";
            postData += $"&scope={scope}";
            postData += $"&response_type=code";

            var browserUrl = $"{url}?{postData}";

            return browserUrl;
        }

        public void ReceiveAccessToken(string code)
        {
            // https://developers.google.com/identity/protocols/OAuth2InstalledApp

            var redirectURI = "urn:ietf:wg:oauth:2.0:oob";
            code = WebUtility.UrlEncode(code);

            try
            {
                var url = "https://www.googleapis.com/oauth2/v4/token";

                string postData = $"code={code}";
                postData += $"&client_id={AuthInfo.client_id}";
                postData += $"&client_secret={AuthInfo.client_secret}";
                postData += $"&redirect_uri={redirectURI}";
                postData += $"&grant_type=authorization_code";

                AccessToken = SendRequest<GAPIAccessToken>(url, postData,"POST", null);
                AccessToken.expires_at = DateTime.Now.AddSeconds(Convert.ToDouble(AccessToken.expires_in));
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex);
                throw;
            }
        }

        public void RefreshAccessToken()
        {
            // https://developers.google.com/identity/protocols/OAuth2InstalledApp

            var refreshTokenFix = WebUtility.UrlEncode(AccessToken.refresh_token);

            try
            {
                var url = "https://www.googleapis.com/oauth2/v4/token";

                string postData = $"refresh_token={AccessToken.refresh_token_urlencoded}";
                postData += $"&client_id={AuthInfo.client_id}";
                postData += $"&client_secret={AuthInfo.client_secret}";
                postData += $"&grant_type=refresh_token";

                AccessToken = SendRequest<GAPIAccessToken>(url, postData, "POST");
                AccessToken.expires_at = DateTime.Now.AddSeconds(Convert.ToDouble(AccessToken.expires_in));
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex);
                throw;
            }
        }


        public static T SendRequest<T>(string url,
                GAPIBaseObject obj,
                string accessToken = null)
        {
            return SendRequest<T>(url, JsonConvert.SerializeObject(obj), "POST", accessToken, "application/json");
        }

        /// <summary>
        /// Sends the request.
        /// </summary>
        /// <returns>The request.</returns>
        /// <param name="request">Request.</param>
        /// <param name="data">POST Data</param>
        /// <param name="accessToken">Access token.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private static string SendRequestBase(HttpWebRequest request,
               string data,
               string accessToken = null)
        {
            if (request.Method == "GET")
            {
                Logger.WriteToLog($"Sending GET Grequest to url: {request.RequestUri}?{data}");
            }
            else
            {
                Logger.WriteToLog($"Sending POST request to url: {request.RequestUri}");
                Logger.WriteToLog($"Posting data length: {data.Length}");
                if (request.ContentType != "application/octet-stream")
                {
                    Logger.WriteToLog($"Posting data: {data}");
                }
            }

            Logger.WriteToLog($"ContentType: {request.ContentType}");

            if (!String.IsNullOrEmpty(accessToken))
            {
                accessToken = WebUtility.UrlEncode(accessToken);
                request.Headers.Add("Authorization", "Bearer " + accessToken);
                request.PreAuthenticate = true;
                request.Accept = "application/json";
            }

            if (request.Method == "POST")
            {
                var postData = string.IsNullOrEmpty(data)
                ? new byte[0]
                : Encoding.ASCII.GetBytes(data);

                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(postData, 0, data.Length);
                }
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Logger.WriteToLog("Response:");
            Logger.WriteToLog(responseString);

            //return (T)Convert.ChangeType(obj, typeof(T));

            return responseString;
        }

        public static T SendRequest<T>(string url,
                string data,
                string method,
                string accessToken = null,
                string contentType = "application/x-www-form-urlencoded")
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = method;
            request.ContentType = contentType;

            var responseString = SendRequestBase(request, data, accessToken);

            return JsonConvert.DeserializeObject<T>(responseString);
        }


        public string UploadFile(string fileName)
        {
            Logger.WriteToLog($"Uploading file {fileName}");

            // https://developers.google.com/photos/library/guides/upload-media

            var url = "https://photoslibrary.googleapis.com/v1/uploads";

            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/octet-stream";
            request.Headers["X-Goog-Upload-File-Name"] = Path.GetFileName(fileName);
            request.Headers["X-Goog-Upload-Protocol"] = "raw";

            string data = new StreamReader(File.Open(fileName, FileMode.Open)).ReadToEnd();

            string responseString = SendRequestBase(request, data, AccessToken.access_token);

            Logger.WriteToLog($"Request result: {responseString}");

            return responseString;
        }
    }
}

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
                    Logger.WriteToLog("Empty refresh token");

                    Authenticate();
                }
                else
                {
                    if (AccessToken.expires_at < DateTime.Now)
                    {
                        Logger.WriteToLog($"Access token expired at [{AccessToken.expires_at.ToString()}]");

                        RefreshAccessToken();
                    }
                }
            }
            else
            {
                Logger.WriteToLog("Access token does not exist");

                Authenticate(); 
            }

            Logger.WriteToLog($"Access token will expire at [{AccessToken.expires_at.ToString()}]");
        }

        public void Authenticate()
        {
            // authenticate user & user consent

            var browserUrl = GetUrlForAuthCode();

            Logger.WriteToLog("Authorize on url:");
            Logger.WriteToLog();
            Logger.WriteToLog(browserUrl);
            Logger.WriteToLog();

            Console.Write("Paste auth code:");
            var code = Console.ReadLine();

            ReceiveAccessToken(code);
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

                AccessToken.SaveToFile("token.json");
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

                AccessToken.SaveToFile("token.json");
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
               string ASCIIPOSTdata,
               string accessToken = null)
        {
            if (request.Method == "GET")
            {
                Logger.WriteToLog($"Sending GET request to url: {request.RequestUri}");
            }
            else
            {
                Logger.WriteToLog($"Sending POST request to url: {request.RequestUri}");

                if (request.ContentLength >= 0)
                {
                    Logger.WriteToLog($"Request ContentLength: {request.ContentLength}");
                }

                if (ASCIIPOSTdata != null)
                {
                    Logger.WriteToLog($"Posting data length: {ASCIIPOSTdata.Length}");

                    if (request.ContentType != "application/octet-stream")
                    {
                        Logger.WriteToLog($"Posting data: {ASCIIPOSTdata}");
                    }
                }
            }

            Logger.WriteToLog($"ContentType: {request.ContentType}");

            if (!String.IsNullOrEmpty(accessToken))
            {
                accessToken = WebUtility.UrlEncode(accessToken);
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

            Logger.WriteToLog($"Method: {request.Method}");
            Logger.WriteToLog($"RequestUri: {request.RequestUri}");
            Logger.WriteToLog($"ContentType: {request.ContentType}");
            Logger.WriteToLog($"ContentLength: {request.ContentLength}");

            foreach (var header in request.Headers)
            {
                Logger.WriteToLog($"Header: {header.ToString()}");
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Logger.WriteToLog($"Response: {responseString}");
            Logger.WriteToLog();
            Logger.WriteToLog($"StatusCode: {response.StatusCode}");
            Logger.WriteToLog($"StatusDescription: {response.StatusDescription}");

            Logger.WriteToLog($"ContentLength: {response.ContentLength}");
            Logger.WriteToLog($"ContentType: {response.ContentType}");
            Logger.WriteToLog($"ContentEncoding: {response.ContentEncoding}");

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
            request.Accept = "application/json";

            var responseString = SendRequestBase(request, data, accessToken);

            return JsonConvert.DeserializeObject<T>(responseString);
        }

        /// <summary>
        /// Upload files.
        /// </summary>
        /// <returns>Key .. uploadToken, value .. filename</returns>
        /// <param name="fileNames">File names.</param>
        public Dictionary<string, string> UploadFiles(List<string> fileNames)
        {
            var result = new Dictionary<string,string>();
            foreach (var filename in fileNames)
            {
                result.Add(UploadFile(filename),filename);
            }

            return result;
        }

        public string UploadFile(string fileName)
        {
            try
            {
                Logger.WriteToLog($"Uploading file {fileName}");

                // https://developers.google.com/photos/library/guides/upload-media

                var url = "https://photoslibrary.googleapis.com/v1/uploads";

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/octet-stream";
                request.Headers["X-Goog-Upload-File-Name"] = Path.GetFileName(fileName);
                request.Headers["X-Goog-Upload-Protocol"] = "raw";

                // fill request stream buffer with file
                var bytesRead = 0;
                byte[] buffer = new byte[1024];
                var requestStream = request.GetRequestStream();
                using (var fs = System.IO.File.OpenRead(fileName))
                {
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }
                }

                var responseString = SendRequestBase(request, null, AccessToken.access_token);

                return responseString;               

            } catch (Exception ex)
            {
                Logger.WriteToLog(ex);
                throw;
            }
        }
    }
}

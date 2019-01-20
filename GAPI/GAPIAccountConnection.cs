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
                    Logger.Info("Empty refresh token");

                    Authenticate();
                }
                else
                {
                    if (AccessToken.expires_at < DateTime.Now)
                    {
                        Logger.Info($"Access token expired at [{AccessToken.expires_at.ToString()}]");

                        RefreshAccessToken();
                    }
                }
            }
            else
            {
                Logger.Info("Access token does not exist");

                Authenticate(); 
            }

            Logger.Info($"Access token will expire at [{AccessToken.expires_at.ToString()}]");
        }

        public void Authenticate()
        {
            // authenticate user & user consent
            Logger.Info("Authenticating user");

            var browserUrl = GetUrlForAuthCode();

            Console.WriteLine("Authorize on url:");
            Console.WriteLine();
            Console.WriteLine(browserUrl);
            Console.WriteLine();

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
            Logger.Info("Receiving access token");

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
                Logger.Error(ex);
                throw;
            }
        }

        public void RefreshAccessToken()
        {
            Logger.Info("Refreshing access token");

            // https://developers.google.com/identity/protocols/OAuth2InstalledApp

            var refreshTokenFix = WebUtility.UrlEncode(AccessToken.refresh_token);

            try
            {
                var url = "https://www.googleapis.com/oauth2/v4/token";

                string postData = $"refresh_token={AccessToken.refresh_token_urlencoded}";
                postData += $"&client_id={AuthInfo.client_id}";
                postData += $"&client_secret={AuthInfo.client_secret}";
                postData += $"&grant_type=refresh_token";

                var refreshedAccessToken = SendRequest<GAPIAccessToken>(url, postData, "POST");

                AccessToken.access_token = refreshedAccessToken.access_token;
                AccessToken.expires_at = DateTime.Now.AddSeconds(Convert.ToDouble(refreshedAccessToken.expires_in));

                AccessToken.SaveToFile("token.json");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
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
        /// <returns>Response string</returns>
        /// <param name="request">Request.</param>
        /// <param name="ASCIIPOSTdata">POST Data</param>
        /// <param name="accessToken">Access token.</param>
        private static string SendRequestBase(HttpWebRequest request,
               string ASCIIPOSTdata,
               string accessToken = null)
        {
            if (request.Method == "GET")
            {
                Logger.Info($"Sending GET request to url: {request.RequestUri}");
            }
            else
            {
                Logger.Info($"Sending POST request to url: {request.RequestUri}");

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

            Logger.Info($"ContentType: {request.ContentType}");

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

            Logger.Debug($"Method: {request.Method}");
            Logger.Debug($"RequestUri: {request.RequestUri}");
            Logger.Debug($"ContentType: {request.ContentType}");
            Logger.Debug($"ContentLength: {request.ContentLength}");

            foreach (var header in request.Headers)
            {
                Logger.Debug($"Header: {header.ToString()}");
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Logger.Debug($"Response: {responseString}");
            Logger.Debug($"StatusCode: {response.StatusCode}");
            Logger.Debug($"StatusDescription: {response.StatusDescription}");

            Logger.Debug($"ContentLength: {response.ContentLength}");
            Logger.Debug($"ContentType: {response.ContentType}");
            Logger.Debug($"ContentEncoding: {response.ContentEncoding}");

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

        public string UploadFile(string fileName)
        {
            try
            {
                Logger.Info($"Uploading file {fileName}");

                // https://developers.google.com/photos/library/guides/upload-media

                var url = "https://photoslibrary.googleapis.com/v1/uploads";

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/octet-stream";

                var fNameValidForHeader = NormalizeStringForUrl(Path.GetFileNameWithoutExtension(fileName)) +
                    "." + NormalizeStringForUrl(Path.GetExtension(fileName));
                request.Headers["X-Goog-Upload-File-Name"] = fNameValidForHeader; // non ascii characters cause "System.ArgumentException: Specified value has invalid CRLF characters."
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

                Logger.Info("Posting file data");
                var responseString = SendRequestBase(request, null, AccessToken.access_token);

                return responseString;               

            } catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}

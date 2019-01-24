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
            try
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

                Logger.Debug($"Access token will expire at [{AccessToken.expires_at.ToString()}]");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public void Authenticate()
        {
            try
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
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
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
            try
            {
                Logger.Info("Receiving access token");

                // https://developers.google.com/identity/protocols/OAuth2InstalledApp

                var redirectURI = "urn:ietf:wg:oauth:2.0:oob";
                code = WebUtility.UrlEncode(code);

                var url = "https://www.googleapis.com/oauth2/v4/token";

                string postData = $"code={code}";
                postData += $"&client_id={AuthInfo.client_id}";
                postData += $"&client_secret={AuthInfo.client_secret}";
                postData += $"&redirect_uri={redirectURI}";
                postData += $"&grant_type=authorization_code";

                AccessToken = GAPICommunication.SendRequest<GAPIAccessToken>(url, postData, "POST", null);
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
            try
            {
                Logger.Info("Refreshing access token");

                // https://developers.google.com/identity/protocols/OAuth2InstalledApp

                var refreshTokenFix = WebUtility.UrlEncode(AccessToken.refresh_token);

                var url = "https://www.googleapis.com/oauth2/v4/token";

                string postData = $"refresh_token={AccessToken.refresh_token_urlencoded}";
                postData += $"&client_id={AuthInfo.client_id}";
                postData += $"&client_secret={AuthInfo.client_secret}";
                postData += $"&grant_type=refresh_token";

                var refreshedAccessToken = GAPICommunication.SendRequest<GAPIAccessToken>(url, postData, "POST");

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
                Logger.Info($"Uploading file {fileName}");

                // https://developers.google.com/photos/library/guides/upload-media

                var url = "https://photoslibrary.googleapis.com/v1/uploads";

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/octet-stream";

                var fNameValidForHeader = GAPICommunication.NormalizeStringForUrl(Path.GetFileNameWithoutExtension(fileName)) +
                    "." + GAPICommunication.NormalizeStringForUrl(Path.GetExtension(fileName));
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

                Logger.Debug("Posting file data");
                var responseString = GAPICommunication.SendRequest(request, this);

                return responseString;

            } catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}

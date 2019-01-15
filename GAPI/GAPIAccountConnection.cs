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

            var tokenPath = $"{GAPIBaseObject.AppDataDir}token.json";

            if (File.Exists(tokenPath))
            {
                AccessToken = GAPIBaseObject.LoadFromFile<GAPIAccessToken>("token.json");

                if (AccessToken.expires_at < DateTime.Now)
                {
                    RefreshAccessToken();
                    AccessToken.SaveToFile("token.json");
                }
            }
            else
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

                var responseString = SendRequest(url, postData,"GET", null);

                AccessToken = JsonConvert.DeserializeObject<GAPIAccessToken>(responseString);
                AccessToken.expires_at = DateTime.Now.AddSeconds(Convert.ToDouble(AccessToken.expires_in));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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

                var responseString = SendRequest(url, postData, "GET");

                AccessToken = JsonConvert.DeserializeObject<GAPIAccessToken>(responseString);
                AccessToken.expires_at = DateTime.Now.AddSeconds(Convert.ToDouble(AccessToken.expires_in));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static string SendRequest(string url, 
                string data, 
                string method,
                string accessToken = null,
                string contentType = "application/x-www-form-urlencoded")
        {
            var browserUrl = $"{url}?{data}";

            if (method == "GET")
            {
                Console.WriteLine($"Sending GET Grequest to url: {url}?{data}");
            } else
            {
                Console.WriteLine($"Sending POST request to url: {url}");
                Console.WriteLine($"Posting: {data}");
            }
            Console.WriteLine();

            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = method;
            request.ContentType = "contentType";

            if (!String.IsNullOrEmpty(accessToken))
            {
                accessToken = WebUtility.UrlEncode(accessToken);
                request.Headers.Add("Authorization", "Bearer " + accessToken);
                request.PreAuthenticate = true;
                request.Accept = "application/json";
            }

            if (method == "POST")
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

            Console.WriteLine("Response:");
            Console.WriteLine(responseString);
            Console.WriteLine();

            return responseString;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using GAPI;

namespace TestConsole
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicy) =>
            {
                // workaround for mono linux System.Net.WebException: Error: TrustFailure (Authentication failed, see inner exception.) ---> System.Security.Authentication.AuthenticationException: Authentication failed, see inner exception. ---> Mono.Btls.MonoBtlsException: Ssl error:1000007d:SSL routines:OPENSSL_internal:CERTIFICATE_VERIFY_FAILED
                return true;
            };

            // loading client_id, client_secret and all scopes:
            GAPIClientAuthorizationInfo authInfo;

            var tokenPath = $"{GAPIBaseObject.AppDataDir}authInfo.json";
            if (File.Exists(tokenPath))
            {
                authInfo = GAPIBaseObject.LoadFromFile<GAPIClientAuthorizationInfo>("authInfo.json");
            }
            else
            {
                Console.WriteLine($"Missing {tokenPath}");
                return;
            }

            var accountConn = new GAPIAccountConnection(authInfo);

            //GAPIAlbumsList.GetAlbums(accountConn.AccessToken);
            //GAPIAlbum.GetAlbum(accountConn.AccessToken, "ALRvDKd8KyuLfNh6EL0ZUQB1bmO_nOAEdj0hxnJ1_f_Jxo2LJ3qu05qiqWd7cZVZyZOoZAsGbitZ");
            var alb = GAPIAlbum.CreateAlbum(accountConn.AccessToken, "my new album");
            alb.SaveToFile("newAlb.json");

            Console.WriteLine("Finish.");
            Console.ReadLine();
        }
    }
}

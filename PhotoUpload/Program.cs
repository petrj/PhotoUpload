using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using GAPI;
using System.Text;

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
            accountConn.Connect();

            //var allAlbs = GAPIAlbumsList.GetAllAlbums(accountConn.AccessToken);
            //Console.WriteLine($"All albums total count: {allAlbs.albums.Count.ToString()}");
            //GAPIAlbum.GetAlbum(accountConn.AccessToken, "ALRvDKd8KyuLfNh6EL0ZUQB1bmO_nOAEdj0hxnJ1_f_Jxo2LJ3qu05qiqWd7cZVZyZOoZAsGbitZ");
            //var alb = GAPIAlbum.CreateAlbum(accountConn.AccessToken, "7th labum created from GAPI");
            //alb.SaveToFile("newAlb.json");

            var fileToken = accountConn.UploadFile(@"c:\temp\foto.jpg");
            var itemsToUpoad = new Dictionary<string, string>() { { fileToken, "foto.jpg" } };
            var uploadedItems = GAPIAlbum.AddMediaItemsToAlbum(accountConn.AccessToken, "ALRvDKcpzu2dtJT3mjS1JYOonliG0pvScB3RuCVzq2qNxDynvDCzwnA-RPNM5hQKq_032_VEQdtu", itemsToUpoad);
            uploadedItems.SaveToFile("uploadedItems.json");

            Console.WriteLine();
            Console.WriteLine("Process finished. Press Enter.......");
            Console.ReadLine();
        }
    }
}

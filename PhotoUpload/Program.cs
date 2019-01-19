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
                Logger.Info($"Missing {tokenPath}");
                return;
            }

            var accountConn = new GAPIAccountConnection(authInfo);
            accountConn.Connect();

            var allAlbs = GAPIAlbumsList.GetAllAlbums(accountConn.AccessToken);
            Logger.Info($"All albums total count: {allAlbs.albums.Count.ToString()}");
            //GAPIAlbum.GetAlbum(accountConn.AccessToken, "ALRvDKd8KyuLfNh6EL0ZUQB1bmO_nOAEdj0hxnJ1_f_Jxo2LJ3qu05qiqWd7cZVZyZOoZAsGbitZ");

            //UploadFolderToAlbum(accountConn, new DirectoryInfo("/temp/19xx/199x_Gympl_scan"));

            Logger.Info();

            Console.WriteLine("Process finished. Press Enter.......");
            Console.ReadLine();
        }

        public static void UploadFolderToAlbum(GAPIAccountConnection accountConn, DirectoryInfo directory, string albumName = null)
        {
            if (albumName == null)
                albumName = directory.Name;

            var alb = GAPIAlbum.CreateAlbum(accountConn.AccessToken, albumName);

            var files = directory.GetFiles();
            var fileNames = new List<string>();
            foreach (var f in files)
            {
                fileNames.Add(f.FullName);
            }

            fileNames.Sort();

            var fileTokens = accountConn.UploadFiles(fileNames);

            if (fileTokens != null)
            {
                var uploadedItems = GAPIAlbum.AddMediaItemsToAlbum(accountConn.AccessToken, alb.id, fileTokens);

            }
            else
            {
                Logger.Error("Upload failed - empty fileTokens");
            }
        }
    }
}

using GAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        public static string AuthInfoPath
        {
            get
            {
                return $"{GAPIBaseObject.AppDataDir}authInfo.json";
            }
        }

        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicy) =>
            {
                // workaround for mono linux System.Net.WebException: Error: TrustFailure (Authentication failed, see inner exception.) ---> System.Security.Authentication.AuthenticationException: Authentication failed, see inner exception. ---> Mono.Btls.MonoBtlsException: Ssl error:1000007d:SSL routines:OPENSSL_internal:CERTIFICATE_VERIFY_FAILED
                return true;
            };

            if (!File.Exists(AuthInfoPath))
            {
                Logger.Error($"Missing {AuthInfoPath}");
                Console.ReadLine();
                return;
            }

            var authInfo = GAPIBaseObject.LoadFromFile<GAPIClientAuthorizationInfo>(AuthInfoPath);

            var accountConnection = new GAPIAccountConnection(authInfo);
            accountConnection.Connect();

            var voidedPurchases = VoidedPurchasesList.GetVoidedPurchases(accountConnection, "net.petrjanousek.OnlineTelevizor");

            //accountConnection.RefreshAccessToken();
            //var albs = GAPIAlbumsList.GetAllAlbums(accountConnection);

            //var newAlb = GAPIAlbum.CreateAlbum(accountConnection, "Album Y");
            //var newItems = GAPIAlbum.AddMediaItemToAlbum(accountConnection, newAlb.id, @"/temp/video.mkv");

            /*
            var about = GAPIAbout.AboutUser(accountConnection);

            Console.WriteLine($"Account name: {about.name}");

            var albs = GAPIAlbumsList.GetAllAlbums(accountConnection);

            Console.WriteLine($"Albums");
            Console.WriteLine($"---------------------------------");
            Console.WriteLine($"id,count,title");
            foreach (var alb in albs.albums)
            {
                Console.WriteLine($"{alb.id},{alb.mediaItemsCount,5},{alb.title}");
            }

            var items = GAPIItemsList.GetAllItems(accountConnection);

            Console.WriteLine($"Items");
            Console.WriteLine($"---------------------------------");
            Console.WriteLine($"id,filename,description");
            foreach (var item in items.mediaItems)
            {
                Console.WriteLine($"{item.id},{item.filename},{item.description}");
            }

            */
            Console.WriteLine("Process finished. <ENTER>");
            Console.ReadLine();
        }
    }
}

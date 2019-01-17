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
            accountConn.Connect();

            //var allAlbs = GAPIAlbumsList.GetAllAlbums(accountConn.AccessToken);
            //Console.WriteLine($"All albums total count: {allAlbs.albums.Count.ToString()}");
            //GAPIAlbum.GetAlbum(accountConn.AccessToken, "ALRvDKd8KyuLfNh6EL0ZUQB1bmO_nOAEdj0hxnJ1_f_Jxo2LJ3qu05qiqWd7cZVZyZOoZAsGbitZ");
            //var alb = GAPIAlbum.CreateAlbum(accountConn.AccessToken, "7th labum created from GAPI");
            //alb.SaveToFile("newAlb.json");

            //var fileToken = accountConn.UploadFile("/temp/S0415.JPG");

            var itemsToUpoad = new Dictionary<string, string>() { { @"CAIS6QIAMbP+LfaB8gauQBXDq4SwIIifInCKoqwZds0oLBj5dhspsZVfRQcm7SOHmBFnBieFq1pnaTi/ELw5uHRXnnCBAmyJtqwFd2KVY0jNBlBPrXxo+uOCUAPcwUWjDqFjJeVYt7yS4YK1M/LdVg/3TKvDmsRguF4T3Cpnvbwf5MdsQ8qAncZYd5xSaGiBoqk1iDIcGgWNaGedjvqVSaN3INDJQL3Jr60i+q9p1j8ABBiOBe97dzg/cuhx1q1DasjJi5ALu5kf1e3LAQmjUdW74f9vFX9Zjm8YUh+EKChGXsZU9lIUPXmxc77SxcGhhN0n5hXT8le6/aJmz57UoM23ipJvId+BE3BdpNnSTrwlMagc5satX5LUF3KS7zsmKaCFUVrz0sp1buz083Tn9QIwYb/o9f3fmoQWREekqp5TRKxsV7r5MLGF0LwF3rWwqAYwE7zD2WdUZ8HL6dcKpNPiFCeMYRNWAHqUs5Aa", "S0415.JPG" } };
            var uploadedItems = GAPIAlbum.AddMediaItemsToAlbum(accountConn.AccessToken, "ALRvDKcpzu2dtJT3mjS1JYOonliG0pvScB3RuCVzq2qNxDynvDCzwnA-RPNM5hQKq_032_VEQdtu", itemsToUpoad);
            uploadedItems.SaveToFile("uploadedItems.json");

            Console.WriteLine();
            Console.WriteLine("Process finished. Press Enter.......");
            Console.ReadLine();
        }
    }
}

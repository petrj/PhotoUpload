using System;
using System.IO;
using System.Net;
using GAPI;

namespace googleInfo
{
    class MainClass
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

            var about = GAPIAbout.AboutUser(accountConnection);

            if ((args.Length == 1) && (args[0] == "--json"))
            {
                Console.WriteLine(about.ToString());
            }
            else
            {
                Logger.Info($"Name           : {about.name}");
                Logger.Info($"Email          : {about.user.emailAddress}");
                Logger.Info();
                Logger.Info($"Total Size (GB): {about.GBytesTotal.PadLeft(8,' ')}");
                Logger.Info($"Used Size  (GB): {about.GBytesUsedByAppsTotal.PadLeft(8, ' ')}");
                Logger.Info($"Free Size  (GB): {about.GBytesFreeTotal.PadLeft(8, ' ')}");
                Logger.Info();
                Logger.Info("Used size by service (GB):");
                foreach (var sq in about.quotaBytesByService)
                {
                    Logger.Info($"{sq.serviceName.PadRight(15,' ')}: {sq.GBytesUsed.PadLeft(8, ' ')}");
                }
            }
        }
    }
}
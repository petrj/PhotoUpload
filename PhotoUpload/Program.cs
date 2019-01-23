using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using GAPI;
using System.Text;

namespace PhotoUpload
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

            if (args.Length == 0)
            {
                Console.WriteLine("Missing DirectoryName argument");
                return;
            }

            if (args.Length > 2)
            {
                Console.WriteLine("Too many arguments");
                return;
            }

            string directory = null;
            bool reupload = false;

            foreach (var arg in args)
            {
                if (arg == "--reupload")
                {
                    reupload = true;
                }
                else
                {
                    directory = arg;
                }
            }

            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"Directory {directory} does not exist");
                return;
            }

            var photoUpload = new PhotoUpload();
            photoUpload.Connect();

            photoUpload.UploadFolders(new DirectoryInfo(directory), reupload);

            Logger.Info();

            Console.WriteLine("Process finished.");
        }
    }
}

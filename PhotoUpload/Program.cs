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
    
            var photoUpload = new PhotoUpload();
            photoUpload.Connect();

            //photoUpload.UploadFolderToAlbum(new DirectoryInfo("/temp/PATHFIND"));
            photoUpload.UploadFolders(new DirectoryInfo("/temp/"));

            Logger.Info();

            Console.WriteLine("Process finished. Press Enter.......");
            Console.ReadLine();
        }       
    }
}

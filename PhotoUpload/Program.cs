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
                Console.WriteLine("Missing arguments, see \"PhotoUpload.exe --help\" ");
                return;
            }

            string directory = null;

            bool uploadFolder = false;
            bool reupload = false;
            bool info = false;
            bool json = false;
            bool help = false;

            foreach (var arg in args)
            {
                if (ArgDefined("help", arg))
                {
                    help = true;
                } else
                if (ArgDefined("reupload", arg))
                {
                    reupload = true;
                } else
                if (ArgDefined("info", arg))
                {
                    info = true;
                } else
                if (ArgDefined("json", arg))
                {
                    json = true;
                }
                else
                {
                    directory = arg;

                    if (!Directory.Exists(directory))
                    {
                        Console.WriteLine($"Invalid argument: directory {directory} does not exist");
                        return;
                    }

                    uploadFolder = true;
                }
            }

            if (help)
            {
                Help();
                return;
            }

            var photoUpload = new PhotoUpload();
            photoUpload.Connect();

            if (info)
            {
                var about = GAPIAbout.AboutUser(photoUpload.AccountConnection);

                if (json)
                {
                    Console.WriteLine(about.ToString());
                }
                else
                {
                    Logger.Info($"Name           : {about.name}");
                    Logger.Info($"Email          : {about.user.emailAddress}");
                    Logger.Info();
                    Logger.Info($"Total Size (GB): {about.GBytesTotal.PadLeft(8, ' ')}");
                    Logger.Info($"Used Size  (GB): {about.GBytesUsedByAppsTotal.PadLeft(8, ' ')}");
                    Logger.Info($"Trash Size (GB): {about.GBytesTrashTotal.PadLeft(8, ' ')}");
                    Logger.Info($"Free Size  (GB): {about.GBytesFreeTotal.PadLeft(8, ' ')}");
                    Logger.Info();
                    Logger.Info("Used size by service (GB):");

                    foreach (var sq in about.quotaBytesByService)
                    {
                        Logger.Info($"{sq.serviceName.PadRight(15, ' ')}: {sq.GBytesUsed.PadLeft(8, ' ')}");
                    }
                }
            }

            if (uploadFolder)
            {
                photoUpload.UploadFolders(new DirectoryInfo(directory), reupload);

                Logger.Info();

                Console.WriteLine("Process successfully finished.");
            }
        }

        private static void Help()
        {
            Console.WriteLine("PhotoUpload");
            Console.WriteLine();
            Console.WriteLine("usage:");
            Console.WriteLine();
            Console.WriteLine("  PhotoUpload.exe directoryName");
            Console.WriteLine();
            Console.WriteLine("    - for uploading directoryName recursively");
            Console.WriteLine();
            Console.WriteLine("  PhotoUpload.exe directoryName --reupload");
            Console.WriteLine();
            Console.WriteLine("    - for reuploading directoryName recursively");
            Console.WriteLine("    - directories in journal.json are reuploaded, other directories are uploaded as usually");
            Console.WriteLine();
            Console.WriteLine("  PhotoUpload.exe --info");
            Console.WriteLine();
            Console.WriteLine("    - for showing Google account informations such as name, email and free size on drive");
            Console.WriteLine();
            Console.WriteLine("  PhotoUpload.exe --info --json");
            Console.WriteLine();
            Console.WriteLine("    - for showing allGoogle account informations in json format");
            Console.WriteLine();
            Console.WriteLine("  PhotoUpload.exe --help");
            Console.WriteLine();
            Console.WriteLine("    - for showing this help and exit");
        }

        private static bool ArgDefined(string argName, string argument)
        {
            argName = argName.ToLower();
            argument = argument.ToLower();

            if (
                ("--" + argName == argument) ||
                ("-" + argName == argument) ||
                ("/" + argName == argument)
                )
            {
                return true;
            }

            return false;
        }
    }
}

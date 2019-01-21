using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using GAPI;
using Newtonsoft.Json;

namespace PhotoUpload
{
    public class PhotoUpload
    {
        public List<string> AllowedExtensions { get; set; } = new List<string>() {
            ".jpg" ,
            ".jpeg",
            ".jpgrd",
            ".png",
            ".bmp",
            ".gif",
            ".avi",
            ".mp4",
            ".m4v",
            ".3gp",
            ".mov" };

        private UploadedJournal _uploadedJournal = new UploadedJournal();

        // client_id, client_secret and all scopes
        private GAPIClientAuthorizationInfo _authInfo;
        private GAPIAccountConnection _accountConnection;

        public string JournalPath
        {
            get
            {
                return $"{GAPIBaseObject.AppDataDir}journal.json";
            }
        }

        public string AuthInfoPath
        {
            get
            {
                return $"{GAPIBaseObject.AppDataDir}authInfo.json";
            }
        }

        public PhotoUpload()
        {
            Logger.Debug("Starting PhotoUploader process");
            _authInfo = new GAPIClientAuthorizationInfo();
        }

        public bool Connect()
        {
            try
            {
                var res = true;

                if (File.Exists(AuthInfoPath))
                {
                    _authInfo = GAPIBaseObject.LoadFromFile<GAPIClientAuthorizationInfo>(AuthInfoPath);
                }
                else
                {
                    Logger.Info($"Missing {AuthInfoPath}");
                    res = false;
                }

                if (File.Exists(JournalPath))
                {
                    _uploadedJournal = GAPIBaseObject.LoadFromFile<UploadedJournal>(JournalPath);
                }
                else
                {
                    Logger.Debug($"Missing {JournalPath}");
                }

                if (res)
                {
                    _accountConnection = new GAPIAccountConnection(_authInfo);
                    _accountConnection.Connect();
                }

                return res;

            } catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public void AddToJournal(string directoryName, string albumId)
        {
            try
            {
                var alb = _uploadedJournal.Directory(directoryName);
                if (alb == null)
                {
                    Logger.Info($"Adding folder {directoryName} to journal (id {albumId})");

                    _uploadedJournal.Albums.Add(new UploadedJournalItem()
                    {
                        DirectoryName = directoryName,
                        AlbumId = albumId
                    });

                    _uploadedJournal.SaveToFile(JournalPath);
                }
                else
                {
                    Logger.Info($"Folder {directoryName} is already in journal with {albumId}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public void UploadFolders(DirectoryInfo directory)
        {
            try
            {

                Logger.Info($"Uploading all folders from {directory.Name}");

                // uploading self
                UploadFolderToAlbum(directory);

                var allDirs = directory.GetDirectories("*.*", SearchOption.AllDirectories);

                var dirs = new List<string>();
                foreach (var dir in allDirs)
                {
                    dirs.Add(dir.FullName);
                }

                dirs.Sort();

                foreach (var dir in dirs)
                {
                    UploadFolderToAlbum(new DirectoryInfo(dir));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public void UploadFolderToAlbum(DirectoryInfo directory)
        {
            try
            {
                Logger.Info($"Uploading folder {directory.Name}");

                // looking in journal
                var alreadyUploadedJournalItem = _uploadedJournal.Directory(directory.FullName);
                if (alreadyUploadedJournalItem != null)
                {
                    Logger.Warning($"Folder {directory.FullName} is already uploaded with id {alreadyUploadedJournalItem.AlbumId}");
                    return;
                }

                //Logger.Info($"Searching all files in {directory.Name}");

                // https://stackoverflow.com/questions/7039580/multiple-file-extensions-searchpattern-for-system-io-directory-getfiles/7039649
                var files = Directory.GetFiles(directory.FullName, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => AllowedExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();

                Logger.Info($"Files found: {files.Count}");

                if (files.Count == 0)
                {
                    return;
                }

                files.Sort();

                //  create album

                var alb = GAPIAlbum.CreateAlbum(_accountConnection, directory.Name);

                // batch upload
                var batchCount = 20;

                var fileTokensBatch = new List<Dictionary<string, string>>();

                var actualBatch = new Dictionary<string, string>();

                var fIndex = 0;
                foreach (var f in files)
                {
                    Logger.Info($"Uploading file {f}");

                    //  upload file
                    string uploadToken = _accountConnection.UploadFile(f);

                    if (string.IsNullOrEmpty(uploadToken))
                    {
                        Logger.Warning($"Empty upload file token for file ({f})");
                    }
                    else
                    {
                        actualBatch.Add(uploadToken, Path.GetFileName(f));
                    }

                    fIndex++;
                    if (fIndex > batchCount)
                    {
                        if (actualBatch.Count > 0)
                        {
                            actualBatch = new Dictionary<string, string>();
                            fileTokensBatch.Add(actualBatch);
                        }
                        fIndex = 0;
                    }
                }

                if (actualBatch.Count > 0)
                {
                    fileTokensBatch.Add(actualBatch);
                }

                if (fileTokensBatch.Count > 0)
                {
                    foreach (var batch in fileTokensBatch)
                    {
                        var uploadedItems = GAPIAlbum.AddMediaItemsToAlbum(_accountConnection, alb.id, batch);
                    }

                    AddToJournal(directory.FullName, Guid.NewGuid().ToString());
                }
                else
                {
                    Logger.Info($"No file uploaded");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}

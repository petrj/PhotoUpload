using System;
using GAPI;

namespace PhotoUpload
{
    public class UploadedJournalItem : GAPIBaseObject
    {
        public string DirectoryName { get; set; }
        public string AlbumId { get; set; }
    }
}

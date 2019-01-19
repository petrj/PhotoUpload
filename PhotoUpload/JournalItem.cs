using System;
using GAPI;

namespace PhotoUpload
{
    public class JournalItem : GAPIBaseObject
    {
        public string DirectoryName { get; set; }
        public string AlbumId { get; set; }
    }
}

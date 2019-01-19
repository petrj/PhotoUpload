using System;
using System.Collections.Generic;
using GAPI;


namespace PhotoUpload
{
    public class UploadedJournal : GAPIBaseObject
    {
        public List<JournalItem> Albums { get; set; } = new List<JournalItem>();

        public JournalItem Directory(string directoryName)
        {
            foreach (var alb in Albums)
            {
                if (alb.DirectoryName.ToLower() == directoryName.ToLower())
                {
                    return alb;
                }
            }

            return null;
        }
    }
}

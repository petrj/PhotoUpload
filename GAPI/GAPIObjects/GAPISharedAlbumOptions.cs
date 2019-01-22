using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GAPISharedAlbumOptions : GAPIBaseObject
    {
        // https://developers.google.com/photos/library/reference/rest/v1/albums#Album.SharedAlbumOptions

        public bool isCollaborative { get; set; }
        public bool isCommentable { get; set; }
    }
}

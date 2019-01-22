using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public enum GAPIPositionType
    {
        POSITION_TYPE_UNSPECIFIED ,  //Default value if this enum isn't set.
        FIRST_IN_ALBUM, // At the beginning of the album.
        LAST_IN_ALBUM, // At the end of the album.
        AFTER_MEDIA_ITEM, // After a media item.
        AFTER_ENRICHMENT_ITEM // After an enrichment item.
    }
}

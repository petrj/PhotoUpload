using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GAPINewMediaItemResult : GAPIBaseObject
    {
        public string uploadToken { get; set; }
        public GAPIStatus Status { get; set; }
        public GAPIMediaItem mediaItem { get; set; } = new GAPIMediaItem();
    }
}

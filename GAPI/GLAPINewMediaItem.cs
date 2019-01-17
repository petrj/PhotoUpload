using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GLAPINewMediaItem : GAPIBaseObject
    {
        public string description { get; set; }
        public GAPISimpleMediaItem simpleMediaItem { get; set; } = new GAPISimpleMediaItem();
    }
}

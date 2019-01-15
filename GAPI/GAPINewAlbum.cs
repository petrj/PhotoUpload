using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GAPINewAlbum : GAPIBaseObject
    {
        public GAPIAlbum album { get; set; } = new GAPIAlbum();
    }
}

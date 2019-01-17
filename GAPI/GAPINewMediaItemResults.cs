using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GAPINewMediaItemResults : GAPIBaseObject
    {
        public List<GAPINewMediaItemResult> newMediaItemResults { get; set; } = new List<GAPINewMediaItemResult>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GAPIPhoto : GAPIBaseObject
    {
        public string cameraMake { get; set; }
        public string cameraModel { get; set; }
        public int focalLength { get; set; }
        public int apertureFNumber { get; set; }
        public int isoEquivalent { get; set; }
        public string exposureTime { get; set; }
    }
}

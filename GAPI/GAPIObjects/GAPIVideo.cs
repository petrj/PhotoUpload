using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GAPIVideo : GAPIBaseObject
    {
        public string cameraMake { get; set; }
        public string cameraModel { get; set; }
        public string fps { get; set; }

        //public GAPIVideoProcessingStatus status { get; set; } = new GAPIVideoProcessingStatus();
        public string status { get; set; }
    }
}

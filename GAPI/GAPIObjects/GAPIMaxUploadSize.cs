using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public partial class GAPIMaxUploadSize : GAPIBaseObject
    {
        public string type { get; set; }
        public long size { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public partial class GAPIQuotaBytesByService : GAPIBaseObject
    {
        public string serviceName { get; set; }
        public long bytesUsed { get; set; }
    }
}

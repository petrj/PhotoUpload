using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public partial class GAPIQuotaBytesByService : GAPIBaseObject
    {
        public string GBytesUsed
        {
            get
            {
                return (bytesUsed / Math.Pow(10, 9)).ToString("N2");
            }
        }
    }
}

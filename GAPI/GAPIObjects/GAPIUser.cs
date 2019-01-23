using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public partial class GAPIUser : GAPIBaseObject
    {
        public string kind { get; set; }
        public string displayName { get; set; }

        public GAPIPicture picture { get; set; } = new GAPIPicture();

        public bool isAuthenticatedUser { get; set; }
        public string permissionId { get; set; }
        public string emailAddress { get; set; }
    }
}

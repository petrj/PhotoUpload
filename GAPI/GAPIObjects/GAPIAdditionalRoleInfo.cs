using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public partial class GAPIAdditionalRoleInfo : GAPIBaseObject
    {
        public string type { get; set; }
        public List<GAPIRoleSets> roleSets { get; set; } = new List<GAPIRoleSets>();
    }
}

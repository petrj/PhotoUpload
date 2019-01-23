using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public partial class GAPIRoleSets : GAPIBaseObject
    {
        public string primaryRole { get; set; }
        public List<string> additionalRoles { get; set; } = new List<string>();
    }
}

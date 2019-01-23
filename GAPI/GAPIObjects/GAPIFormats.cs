using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public partial class GAPIFormats : GAPIBaseObject
    {
        public string source { get; set; }
        public List<string> targets { get; set; } = new List<string>();
    }
}

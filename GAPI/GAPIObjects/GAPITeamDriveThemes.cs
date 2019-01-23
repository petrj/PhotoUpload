using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public partial class GAPITeamDriveThemes : GAPIBaseObject
    {
        public string id { get; set; }
        public string backgroundImageLink { get; set; }
        public string colorRgb { get; set; }
    }
}

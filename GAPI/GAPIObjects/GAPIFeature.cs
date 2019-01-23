using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public partial class GAPIFeature : GAPIBaseObject
    {
        public string featureName { get; set; }
        public double featureRate { get; set; }
    }
}

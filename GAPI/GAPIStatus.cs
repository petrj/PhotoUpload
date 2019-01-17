using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPI
{
    public class GAPIStatus : GAPIBaseObject
    {
        public int code { get; set; }
        public string message { get; set; }

        public List<object[]> details { get; set; } = new List<object[]>();
    }
}

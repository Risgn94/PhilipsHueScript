using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PhilipsHueScript
{
    class Config
    {
        public string IPAddress { get; set; }
        public int Seconds { get; set; }
        public string PathToLightData { get; set; }
        public string BridgeKey { get; set; }
        public bool RunIndefinitely { get; set; }

    }
}

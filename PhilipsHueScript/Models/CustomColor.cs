using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Q42.HueApi.ColorConverters.HSB;

namespace PhilipsHueScript.Models
{
    public class CustomColor
    {
        public int Seconds { get; set; }
        public int Saturation { get; set; }
        public int Brightness { get; set; }
        public XYColor XyColor { get; set; }
    }
}

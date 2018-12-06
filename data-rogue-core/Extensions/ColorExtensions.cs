using RLNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Extensions
{
    public static class ColorExtensions
    {
        public static RLColor ToRLColor(this Color color)
        {
            return new RLColor(color.R, color.G, color.B);
        }
    }
}

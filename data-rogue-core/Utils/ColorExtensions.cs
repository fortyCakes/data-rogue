using System.Drawing;
using RLNET;

namespace data_rogue_core.Utils
{
    public static class ColorExtensions
    {
        public static RLColor ToRLColor(this Color color)
        {
            return new RLColor(color.R, color.G, color.B);
        }
    }
}

using RLNET;
using System.Drawing;

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

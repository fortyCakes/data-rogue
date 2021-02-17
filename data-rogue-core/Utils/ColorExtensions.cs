using System;
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

        public static RLColor Gradient(int max, Color fromColor, Color toColor, int value)
        {
            var weight2 = (decimal)value / max;
            var weight1 = 1 - weight2;

            var color = Color.FromArgb(
                red: (int)Math.Floor(fromColor.R * weight1 + toColor.R * weight2),
                green: (int)Math.Floor(fromColor.G * weight1 + toColor.G * weight2),
                blue: (int)Math.Floor(fromColor.B * weight1 + toColor.B * weight2));

            return color.ToRLColor();
        }

        public static string ToHexCode(this Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}

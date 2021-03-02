using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace data_rogue_core.Utils
{
    public static class RectangleUtils
    {
        public static Rectangle Pad(this Rectangle rectangle, Padding padding)
        {
            return new Rectangle(
                rectangle.Left + padding.Left, 
                rectangle.Top + padding.Top, 
                rectangle.Width - padding.Left - padding.Right, 
                rectangle.Height - padding.Top - padding.Bottom);
        }

        public static Rectangle ShrinkFromTop(this Rectangle rectangle, int shrinkBy)
        {
            return new Rectangle(
                rectangle.Left,
                rectangle.Top + shrinkBy,
                rectangle.Width,
                rectangle.Height - shrinkBy);
        }
    }
}

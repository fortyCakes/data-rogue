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
        public static Rectangle PadIn(this Rectangle rectangle, Padding padding)
        {
            return new Rectangle(
                rectangle.Left + padding.Left, 
                rectangle.Top + padding.Top, 
                rectangle.Width - padding.Horizontal, 
                rectangle.Height - padding.Vertical);
        }

        public static Rectangle PadOut(this Rectangle rectangle, Padding padding)
        {
            return new Rectangle(
                rectangle.Left - padding.Left,
                rectangle.Top - padding.Top,
                rectangle.Width + padding.Horizontal,
                rectangle.Height + padding.Vertical);
        }

        public static Size PadOut(this Size size, Padding padding)
        {
            return new Size(size.Width + padding.Horizontal, size.Height + padding.Vertical);
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

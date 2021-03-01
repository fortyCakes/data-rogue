using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace data_rogue_core.Utils
{
    public static class RectanglePaddingHelper
    {
        public static Rectangle Pad(this Rectangle rectangle, Padding padding)
        {
            return new Rectangle(
                rectangle.Left + padding.Left, 
                rectangle.Top + padding.Top, 
                rectangle.Width - padding.Left - padding.Right, 
                rectangle.Height - padding.Top - padding.Bottom);
        }
    }
}

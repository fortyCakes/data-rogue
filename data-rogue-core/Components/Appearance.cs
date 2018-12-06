using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Components
{
    public class Appearance : IEntityComponent
    {
        public char Glyph;
        public Color Color;
        public int ZOrder;
    }
}

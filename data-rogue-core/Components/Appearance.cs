﻿using System.Drawing;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Appearance : IEntityComponent
    {
        public char Glyph;
        public Color Color;
        public int ZOrder;
    }
}

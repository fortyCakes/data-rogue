﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTLineControlRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(LineControl);
        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            // Do not display anything
        }

        protected override Size Measure(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov, Rectangle boundingBox, Padding padding, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            return new Size(0, 2);
        }
    }
}
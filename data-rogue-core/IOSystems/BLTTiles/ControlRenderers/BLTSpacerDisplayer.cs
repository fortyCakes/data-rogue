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
    internal class BLTSpacerDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(Spacer);
        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            // Do not display anything
        }

        protected override Size Measure(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov, Rectangle boundingBox, Padding padding, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            return new Size(4, 4);
        }
    }
}
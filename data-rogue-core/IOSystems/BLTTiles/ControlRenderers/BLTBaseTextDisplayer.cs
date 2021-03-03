using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public abstract class BLTBaseTextDisplayer : BLTControlRenderer
    {
        protected abstract string GetText(IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            RenderText(control.PaddedLeft, control.PaddedTop, control.ActivityIndex, out _, GetText(control, systemContainer, playerFov), control.Color);
        }

        protected override Size Measure(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov, Rectangle boundingBox, Padding padding, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            BLT.Font("text");
            return BLT.Measure(GetText(control, systemContainer, playerFov));
        }
    }
}
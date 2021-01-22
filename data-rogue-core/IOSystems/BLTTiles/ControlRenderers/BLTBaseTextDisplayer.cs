using System.Collections.Generic;
using System.Drawing;
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
            RenderText(control.Position.X, control.Position.Y, out _, GetText(control, systemContainer, playerFov), control.Color);
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            BLT.Font("text");
            return BLT.Measure(GetText(control, systemContainer, playerFov));
        }
    }
}
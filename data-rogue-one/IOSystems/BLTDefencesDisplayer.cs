using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems.BLTTiles;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_one.IOSystems
{
    public class DefencesControl : BaseInfoControl { }

    public class BLTDefencesDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(DefencesControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = control.Position.X;
            var y = control.Position.Y;
            var display = control as IDataRogueInfoControl;

            //RenderBackgroundBox(x, y, GetSizeInternal(spriteManager, control, systemContainer, playerFov), spriteManager);
            BLT.Font("Text");
            BLT.Print(x, y, "Defenses: ");
            BLT.Print(x, y + 4, "AC: ?, SH: ?, EV: ?");
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(64, 8);
        }
    }
}
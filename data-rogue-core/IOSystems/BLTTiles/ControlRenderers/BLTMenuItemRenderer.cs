using System;
using System.Collections.Generic;
using System.Drawing;
using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Menus;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTMenuItemRenderer : BLTBaseTextDisplayer
    {
        public override Type DisplayType => typeof(MenuItem);

        protected override string GetText(IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return (control as MenuItem).Text;
        }

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var menuItem = control as MenuItem;
            if (menuItem.Enabled)
            {
                menuItem.Color = Color.White;
            }
            else
            {
                menuItem.Color = Color.Gray;
            }

            base.DisplayInternal(spriteManager, control, systemContainer, playerFov);
        }
    }
}
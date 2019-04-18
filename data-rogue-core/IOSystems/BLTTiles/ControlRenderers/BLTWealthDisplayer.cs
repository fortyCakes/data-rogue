using System;
using System.Collections.Generic;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTWealthDisplayer : BLTBaseTextDisplayer
    {
        public override Type DisplayType => typeof(WealthControl);

        protected override string GetText(IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;
            var entity = display.Entity;

            var wealthType = display.Parameters;
            return $"{wealthType}: {systemContainer.ItemSystem.CheckWealth(entity, wealthType)}";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTLocationDisplayer : BLTBaseTextDisplayer
    {
        public override Type DisplayType => typeof(LocationControl);
        protected override string GetText(IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var entity = (control as IDataRogueInfoControl)?.Entity;

            var mapName = systemContainer.PositionSystem.CoordinateOf(entity).Key.Key;
            if (mapName.StartsWith("Branch:"))
            {
                mapName = mapName.Substring(7);
            }

            return $"Location: {mapName}";
        }
    }
}
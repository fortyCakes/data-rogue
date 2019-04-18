using System;
using System.Collections.Generic;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTTextDisplayer : BLTBaseTextDisplayer
    {
        public override Type DisplayType => typeof(TextControl);

        protected override string GetText(IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return (control as IDataRogueInfoControl)?.Parameters;
        }
    }
}
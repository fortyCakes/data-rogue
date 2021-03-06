﻿using System;
using System.Collections.Generic;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTNameDisplayer : BLTBaseTextDisplayer
    {
        public override Type DisplayType => typeof(NameControl);

        protected override string GetText(IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var entity = (control as IDataRogueInfoControl)?.Entity;
            return $"Name: {entity?.GetBLTName()}";
        }
    }
}
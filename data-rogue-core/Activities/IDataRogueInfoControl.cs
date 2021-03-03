using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.BLTTiles;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace data_rogue_core.Activities
{

    public interface IDataRogueInfoControl : IDataRogueControl
    {
        IEntity Entity { get; set; }
        string Parameters { get; set; }
    }
}
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.IOSystems;
using RLNET;
using data_rogue_core.Components;

namespace data_rogue_core.Systems.Interfaces
{
    public interface ITargetingSystem
    {
        void GetTarget(IEntity sender, Targeting data, Action<MapCoordinate> callback);

        HashSet<MapCoordinate> TargetableCellsFrom(Targeting data, MapCoordinate playerPosition);
    }
}

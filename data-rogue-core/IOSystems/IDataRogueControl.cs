using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace data_rogue_core.IOSystems
{
    public interface IDataRogueControlRenderer
    {
        Type DisplayType { get; }

        Size GetSize(object handle, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov);

        void Display(object handle, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov);

        IEntity EntityFromMouseData(IDataRogueControl display, ISystemContainer systemContainer, MouseData mouse);
    }
}

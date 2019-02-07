﻿using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Components
{
    public class Portal : IEntityComponent
    {
        public uint? BranchLink;
        public MapCoordinate Destination;
    }
}

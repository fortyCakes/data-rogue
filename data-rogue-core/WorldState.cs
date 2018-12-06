using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.Renderers;
using System;

namespace data_rogue_core
{
    [Serializable]
    public class WorldState
    {
        public WorldState(EntityEngine entityEngine)
        {
            EntityEngine = entityEngine;
        }

        public MapCollection Maps;

        public Entity Player;
    
        public PositionSystem PositionSystem;

        public Map CurrentMap { get; set; }

        public EntityEngine EntityEngine { get; private set; }
    }
}
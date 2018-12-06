﻿using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.Renderers;
using System;
using data_rogue_core.Systems;

namespace data_rogue_core
{
    [Serializable]
    public class WorldState
    {
        public WorldState(IEntityEngineSystem entityEngineSystem)
        {
            EntityEngineSystem = entityEngineSystem;
        }

        public MapCollection Maps;

        public Entity Player;

        public Map CurrentMap { get; set; }

        public IEntityEngineSystem EntityEngineSystem { get; private set; }
    }
}
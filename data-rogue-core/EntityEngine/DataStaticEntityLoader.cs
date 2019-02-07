﻿using data_rogue_core.EntityEngine;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public class DataStaticEntityLoader : BaseStaticEntityLoader
    {
        public override void Load(ISystemContainer systemContainer)
        {
            Load(systemContainer, "Data/Entities/StaticEntities");
        }
        
    }
}
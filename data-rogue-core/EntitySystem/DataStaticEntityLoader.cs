﻿using System;
using System.IO;
using System.Reflection;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;

namespace data_rogue_core
{
    public class DataStaticEntityLoader : BaseStaticEntityLoader
    {
        public override void Load(IEntityEngine engine)
        {
            Load(engine, "Data/Entities/StaticEntities");
        }
        
    }
}
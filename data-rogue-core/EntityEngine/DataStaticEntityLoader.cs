using System;
using System.IO;
using System.Reflection;
using data_rogue_core.Behaviours;
using data_rogue_core.Data;
using data_rogue_core.EntityEngine;

namespace data_rogue_core
{
    public class DataStaticEntityLoader : BaseStaticEntityLoader
    {
        public override void Load(IEntityEngine engine, IBehaviourFactory behaviourFactory)
        {
            Load(engine, behaviourFactory, "Data/Entities/StaticEntities");
        }
        
    }
}
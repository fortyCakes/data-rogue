using System;
using System.IO;
using System.Reflection;
using data_rogue_core.EntityEngine;

namespace data_rogue_core
{
    public class WorldEntityLoader : BaseStaticEntityLoader
    {
        public override void Load(IEntityEngine engine)
        {
            Load(engine, "Data/Entities/World");
        }

    }
}
using System;
using System.IO;
using System.Reflection;
using data_rogue_core.Behaviours;
using data_rogue_core.EntityEngine;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public class WorldEntityLoader : BaseStaticEntityLoader
    {
        public override void Load(ISystemContainer systemContainer)
        {
            Load(systemContainer, "Data/Entities/World");
        }

    }
}
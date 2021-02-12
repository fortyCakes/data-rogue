using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;

namespace data_rogue_core.Components
{

    public abstract class BranchMapGenerationStrategy : IEntityComponent, IBranchMapGenerator
    {
        public abstract List<IMap> Generate(ISystemContainer systemContainer, Branch branchDefinition, IEntity branchEntity, IProgress<string> progress);
    }
}

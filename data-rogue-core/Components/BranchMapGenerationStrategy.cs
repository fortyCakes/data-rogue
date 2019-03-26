using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.World.GenerationStrategies;
using System.Collections.Generic;

namespace data_rogue_core.Components
{

    public abstract class BranchMapGenerationStrategy : IEntityComponent, IBranchMapGenerator
    {
        public abstract List<Map> Generate(ISystemContainer systemContainer, Branch branchDefinition);
    }
}

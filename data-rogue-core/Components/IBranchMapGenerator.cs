using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_core.Components
{
    public interface IBranchMapGenerator
    {
        List<Map> Generate(ISystemContainer systemContainer, Branch branchDefinition);
    }
}
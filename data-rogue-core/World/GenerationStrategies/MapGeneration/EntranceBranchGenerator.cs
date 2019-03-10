using data_rogue_core.Components;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using System.Collections.Generic;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public class EntranceBranchGenerator: BaseBranchGenerator
    {

        public override string GenerationType => "Entrance";

        protected override List<Map> GenerateMaps(ISystemContainer systemContainer, Branch branchDefinition)
        {
            var entranceMap = new StaticMapGenerator(systemContainer, "StaticMaps/entrance.map").Generate($"{branchDefinition.BranchName}:1", Random);
            var entranceMap2 = new StaticMapGenerator(systemContainer, "StaticMaps/entrance2.map").Generate($"{branchDefinition.BranchName}:2", Random);

            return new List<Map> { entranceMap, entranceMap2 };
        }
    }
}
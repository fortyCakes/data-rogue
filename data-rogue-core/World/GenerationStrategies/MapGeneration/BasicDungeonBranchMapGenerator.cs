using data_rogue_core.Components;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using System.Collections.Generic;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public class BasicDungeonBranchMapGenerator: BranchMapGenerationStrategy
    {
        public override List<Map> Generate(ISystemContainer systemContainer, Branch branchDefinition)
        {
            var mapgen = new BasicDungeonMapGenerator(systemContainer);

            var generatedBranchMaps = new List<Map>();

            for (int i = 1; i <= branchDefinition.Depth; i++)
            {
                var map = mapgen.Generate($"{branchDefinition.BranchName}:{i}", systemContainer.Random);

                generatedBranchMaps.Add(map);
            }

            return generatedBranchMaps;
        }
    }
}
using data_rogue_core.Components;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using System.Collections.Generic;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core
{

    public class VaultBasedDungeonBranchMapGenerator: BranchMapGenerationStrategy
    {
        public string FloorCell = "Cell:Empty";
        public string WallCell = "Cell:Wall";
        public int NumberOfVaults = 10;
        public int MaxTries = 50;

        public override List<IMap> Generate(ISystemContainer systemContainer, Branch branchDefinition, IEntity branch)
        {
            var mapgen = new VaultBasedDungeonMapGenerator(systemContainer, FloorCell, WallCell, NumberOfVaults, MaxTries, branch);

            var generatedBranchMaps = new List<IMap>();

            for (int i = 1; i <= branchDefinition.Depth; i++)
            {
                var map = mapgen.Generate($"{branchDefinition.BranchName}:{i}", systemContainer.Random);

                generatedBranchMaps.Add(map);
            }

            return generatedBranchMaps;
        }
    }
}
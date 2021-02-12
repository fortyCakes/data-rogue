using data_rogue_core.Components;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using System.Collections.Generic;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.EntityEngineSystem;
using System;

namespace data_rogue_core
{
    public class BasicDungeonBranchMapGenerator: BranchMapGenerationStrategy
    {
        public string FloorCell = "Cell:Empty";
        public string WallCell = "Cell:Wall";

        public override List<IMap> Generate(ISystemContainer systemContainer, Branch branchDefinition, IEntity branchEntity, IProgress<string> progress)
        {
            var mapgen = new BasicDungeonMapGenerator(systemContainer, FloorCell, WallCell);

            var generatedBranchMaps = new List<IMap>();

            for (int i = 1; i <= branchDefinition.Depth; i++)
            {
                var map = mapgen.Generate($"{branchDefinition.BranchName}:{i}", systemContainer.Random, progress);

                generatedBranchMaps.Add(map);
            }

            return generatedBranchMaps;
        }
    }
}
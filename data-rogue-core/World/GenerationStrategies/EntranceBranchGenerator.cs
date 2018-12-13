using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using System.Collections.Generic;

namespace data_rogue_core
{
    internal class EntranceBranchGenerator : IBranchGenerator
    {
        public string GenerationType => "Entrance";

        public GeneratedBranch Generate(Branch branchDefinition, IEntityEngineSystem engine, string seed)
        {
            var entranceMap = new BasicDungeonMapGenerator(engine).Generate($"{branchDefinition.BranchName}:1", seed);
                //new StaticMapGenerator(engine, "StaticMaps/entrance.map").Generate($"{branchDefinition.BranchName}:1", seed);
            var entranceMap2 = new StaticMapGenerator(engine, "StaticMaps/entrance2.map").Generate($"{branchDefinition.BranchName}:2", seed);

            var generatedBranch = new GeneratedBranch();

            generatedBranch.Maps = new List<Map> { entranceMap, entranceMap2 };

            return generatedBranch;
        }
    }
}
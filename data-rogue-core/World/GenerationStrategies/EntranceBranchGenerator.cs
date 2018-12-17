using System;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public class EntranceBranchGenerator: BaseBranchGenerator
    {

        public override string GenerationType => "Entrance";

        protected override List<Map> GenerateMaps(Branch branchDefinition, IEntityEngineSystem engine)
        {
            var entranceMap = new StaticMapGenerator(engine, "StaticMaps/entrance.map").Generate($"{branchDefinition.BranchName}:1", Random);
            var entranceMap2 = new StaticMapGenerator(engine, "StaticMaps/entrance2.map").Generate($"{branchDefinition.BranchName}:2", Random);
            var generatedBranchMaps = new List<Map> { entranceMap, entranceMap2 };
            return generatedBranchMaps;
        }

        protected override void ExecuteMapGenCommands(GeneratedBranch generatedBranch, Branch branch, IEntityEngineSystem engine, IPositionSystem position, IPrototypeSystem prototypeSystem)
        {

            var newPortal = prototypeSystem.CreateAt("Props:Portal", "branch entrance portal", branch.At(2, 0, 0));
            var stairs1 = prototypeSystem.CreateAt("Props:DownStairs", "down stairs", branch.At(1, 10, 0));
            var stairs2 = prototypeSystem.CreateAt("Props:UpStairs", "down stairs", branch.At(2, 0, 5));
            
        }
    }
}
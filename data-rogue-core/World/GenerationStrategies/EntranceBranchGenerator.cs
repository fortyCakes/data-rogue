using System;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngine;
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

        protected override List<Map> GenerateMaps(Branch branchDefinition, IEntityEngine engine, IPrototypeSystem prototypeSystem)
        {
            var entranceMap = new StaticMapGenerator(engine, prototypeSystem, "StaticMaps/entrance.map").Generate($"{branchDefinition.BranchName}:1", Random);
            var entranceMap2 = new StaticMapGenerator(engine, prototypeSystem, "StaticMaps/entrance2.map").Generate($"{branchDefinition.BranchName}:2", Random);
            var generatedBranchMaps = new List<Map> { entranceMap, entranceMap2 };
            return generatedBranchMaps;
        }

        protected override void CreateEntities(GeneratedBranch generatedBranch, Branch branch, IEntityEngine engine, IPositionSystem positionSystem, IPrototypeSystem prototypeSystem, string seed)
        {
            // none
        }
    }
}
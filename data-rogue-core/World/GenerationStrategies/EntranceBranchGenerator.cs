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

        protected override void AddEntities(GeneratedBranch branch, Branch branchDefinition, IEntityEngineSystem engine, IPositionSystem position)
        {
            var components = new IEntityComponent[]
            {
                new Appearance {Color = Color.Blue, Glyph = '>', ZOrder = 1}, 
                new Portal(),
                new Position {MapCoordinate = new MapCoordinate($"{branchDefinition.BranchName}:2", 0, 0)}
            };

            engine.New("entrance room portal", components);
        }
    }
}
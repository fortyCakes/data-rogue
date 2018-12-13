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
    public class DungeonBranchGenerator: BaseBranchGenerator
    {
        public override string GenerationType => "Dungeon";

        protected override List<Map> GenerateMaps(Branch branchDefinition, IEntityEngineSystem engine)
        {
            var mapgen = new BasicDungeonMapGenerator(engine);

            var generatedBranchMaps = new List<Map>();

            for (int i = 1; i <= branchDefinition.Depth; i++)
            {
                var map = mapgen.Generate($"{branchDefinition.BranchName}:{i}", Random);

                generatedBranchMaps.Add(map);
            }
            
            return generatedBranchMaps;
        }

        protected override void AddEntities(GeneratedBranch branch, Branch branchDefinition, IEntityEngineSystem engine, IPositionSystem position)
        {
            var firstLayer = branch.Maps.First();
            var emptyCell = engine.GetEntityWithName("Cell:Empty");

            var emptyPositions = firstLayer.Cells.Where(c => c.Value == emptyCell).ToList();
            var emptyPosition = Random.PickOne(emptyPositions).Key;

            var components = new IEntityComponent[]
            {
                new Appearance {Color = Color.Blue, Glyph = '>', ZOrder = 1},
                new Portal(),
                new Position {MapCoordinate = emptyPosition}
            };

            engine.New("entrance room portal", components);
        }
    }
}
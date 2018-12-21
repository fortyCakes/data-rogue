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
    public class DungeonBranchGenerator: BaseBranchGenerator
    {
        public override string GenerationType => "Dungeon";

        protected override List<Map> GenerateMaps(Branch branchDefinition, IEntityEngine engine, IPrototypeSystem prototypeSystem)
        {
            var mapgen = new BasicDungeonMapGenerator(engine, prototypeSystem);

            var generatedBranchMaps = new List<Map>();

            for (int i = 1; i <= branchDefinition.Depth; i++)
            {
                var map = mapgen.Generate($"{branchDefinition.BranchName}:{i}", Random);

                generatedBranchMaps.Add(map);
            }
            
            return generatedBranchMaps;
        }

        protected override void CreateEntities(GeneratedBranch generatedBranch, Branch branch, IEntityEngine engine, IPositionSystem position, IPrototypeSystem prototypeSystem, string seed)
        {
            PlaceDefaultEntrancePortal(generatedBranch, branch, engine, position, prototypeSystem);

            PlaceStairs(generatedBranch, engine, position, prototypeSystem);

            PlaceStairs(generatedBranch, engine, position, prototypeSystem);

            PlaceStairs(generatedBranch, engine, position, prototypeSystem);

            foreach (var map in generatedBranch.Maps)
            {
                for (int i = 0; i < 10; i++)
                {
                    PlaceMonster(map, position, prototypeSystem);
                }
            }
        }

        private void PlaceMonster(Map map, IPositionSystem positionSystem, IPrototypeSystem prototypeSystem)
        {
            prototypeSystem.CreateAt("Monster:Goblin", EmptyPositionOn(map, positionSystem, prototypeSystem));
        }
    }
}
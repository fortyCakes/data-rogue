﻿using System;
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

        protected override List<Map> GenerateMaps(ISystemContainer systemContainer, Branch branchDefinition)
        {
            var mapgen = new BasicDungeonMapGenerator(systemContainer);

            var generatedBranchMaps = new List<Map>();

            for (int i = 1; i <= branchDefinition.Depth; i++)
            {
                var map = mapgen.Generate($"{branchDefinition.BranchName}:{i}", Random);

                generatedBranchMaps.Add(map);
            }
            
            return generatedBranchMaps;
        }

        protected override void CreateEntities(ISystemContainer systemContainer, GeneratedBranch generatedBranch, Branch branch)
        {
            PlaceDefaultEntrancePortal(systemContainer, generatedBranch, branch);

            PlaceStairs(systemContainer, generatedBranch);

            PlaceStairs(systemContainer, generatedBranch);

            PlaceStairs(systemContainer, generatedBranch);

            foreach (var map in generatedBranch.Maps)
            {
                for (int i = 0; i < 10; i++)
                {
                    PlaceMonster(systemContainer, map);
                }
            }
        }

        private void PlaceMonster(ISystemContainer systemContainer, Map map)
        {
            systemContainer.PrototypeSystem.CreateAt("Monster:Goblin", EmptyPositionOn(map, systemContainer));
        }
    }
}
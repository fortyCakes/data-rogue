using System;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core
{
    internal class EntranceBranchGenerator : IBranchGenerator
    {


        public string GenerationType => "Entrance";

        private IRandom Random { get; set; }

        public GeneratedBranch Generate(Branch branchDefinition, IEntityEngineSystem engine, string seed)
        {
            Random = new RNG(seed);

            var entranceMap = new StaticMapGenerator(engine, "StaticMaps/entrance.map").Generate($"{branchDefinition.BranchName}:1", seed);
            var entranceMap2 = new StaticMapGenerator(engine, "StaticMaps/entrance2.map").Generate($"{branchDefinition.BranchName}:2", seed);

            var generatedBranch = new GeneratedBranch();

            generatedBranch.Maps = new List<Map> { entranceMap, entranceMap2 };

            Map previousMap = null;
            foreach (var map in generatedBranch.Maps)
            {
                if (previousMap != null)
                {
                    LinkStairs(previousMap, map);
                }

                previousMap = map;
            }

            return generatedBranch;
        }

        private void LinkStairs(Map previousMap, Map map)
        {
            var stairsDown = GetStairs(previousMap, StairDirection.Down);
            var stairsUp = GetStairs(map, StairDirection.Up);

            if (stairsUp.Count != stairsDown.Count)
            {
                throw new ApplicationException("Stair counts do not match between floors");
            }

            while (stairsDown.Count > 0)
            {
                var downStairs = Random.PickOne(stairsDown);
                var upStairs = Random.PickOne(stairsUp);

                downStairs.Value.Get<Portal>().Destination = upStairs.Key;
                upStairs.Value.Get<Portal>().Destination = downStairs.Key;

                stairsDown.Remove(downStairs);
                stairsUp.Remove(upStairs);
            }
        }

        private static List<KeyValuePair<MapCoordinate, IEntity>> GetStairs(Map map, StairDirection direction)
        {
            return map.Cells.Where(c => c.Value.Has<Portal>() && c.Value.Get<Portal>().Direction == direction).ToList();
        }
    }
}
using data_rogue_core.Components;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using System.Collections.Generic;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.EntityEngineSystem;
using System.Linq;
using System;

namespace data_rogue_core
{

    public class StaticBranchMapGenerator: BranchMapGenerationStrategy
    {
        private readonly string STATIC_MAP_ROOT = "StaticMaps/";
        public string StaticMaps;

        public override List<Map> Generate(ISystemContainer systemContainer, Branch branchDefinition)
        {
            var maps = new List<Map>();
            int floor = 1;

            foreach (string mapName in StaticMaps.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()))
            {
                var generator = new StaticMapGenerator(systemContainer, STATIC_MAP_ROOT + mapName);
                var map = generator.Generate($"{branchDefinition.BranchName}:{floor++}", systemContainer.Random);

                maps.Add(map);
            }

            return maps;
        }
    }
}
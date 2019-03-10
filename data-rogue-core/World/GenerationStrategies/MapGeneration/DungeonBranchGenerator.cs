using data_rogue_core.Components;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using System.Collections.Generic;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Utils;

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

        protected override void CreateEntities(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branchEntity)
        {
            base.CreateEntities(systemContainer, generatedBranch, branchEntity);

            var branch = branchEntity.Get<Branch>();

            PlaceDefaultEntrancePortal(systemContainer, generatedBranch, branch);

            PlaceStairs(systemContainer, generatedBranch);

            PlaceStairs(systemContainer, generatedBranch);

            PlaceStairs(systemContainer, generatedBranch);
        }
    }
}
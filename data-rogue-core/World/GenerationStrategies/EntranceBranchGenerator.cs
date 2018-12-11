using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using System.Collections.Generic;

namespace data_rogue_core
{
    internal class EntranceBranchGenerator : IBranchGenerator
    {
        public string GenerationType => "Entrance";

        public GeneratedBranch Generate(Branch branchDefinition, IEntityEngineSystem engine)
        {
            var entranceMap = MapSerializer.Deserialize(DataFileLoader.LoadFile("StaticMaps/entrance.map"), engine, $"{branchDefinition.BranchName}:1");

            var generatedBranch = new GeneratedBranch();

            generatedBranch.Maps = new List<Map> { entranceMap };

            return generatedBranch;
        }
    }
}
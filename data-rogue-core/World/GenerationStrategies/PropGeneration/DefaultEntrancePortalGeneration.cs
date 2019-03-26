using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.World.GenerationStrategies
{

    public class DefaultEntrancePortalGeneration : BaseEntityGenerationStrategy
    {
        private class ItemList: Dictionary<int, HashSet<IEntity>> { }

        public override void Generate(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branch, IRandom random)
        {
            BranchGenerator.PlaceDefaultEntrancePortal(systemContainer, generatedBranch, random);
        }
        
    }
}

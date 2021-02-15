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

    public class PlaceSpecificItem : BaseEntityGenerationStrategy, IEntityGenerator
    {
        public string Item;
        public int Floor;

        private class ItemList: Dictionary<int, HashSet<IEntity>> { }

        public override void Generate(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branch, IRandom random, IProgress<string> progress)
        {
            progress.Report("Placing specific items");
            SpawnItemRandomly(generatedBranch.Maps[Floor-1], systemContainer, Item, random);
        }

        private void SpawnItemRandomly(IMap map, ISystemContainer systemContainer, string item, IRandom random)
        {
            int retries = 25;

            MapCoordinate emptyLocation = null;

            for (int i = 0; i < retries; i++)
            {
                emptyLocation = map.GetQuickEmptyPosition(systemContainer.PositionSystem, random);

                if (emptyLocation != null) break;
            }

            if (emptyLocation == null || item == null) return;

            systemContainer.PrototypeSystem.CreateAt(item, emptyLocation);
        }
    }
}

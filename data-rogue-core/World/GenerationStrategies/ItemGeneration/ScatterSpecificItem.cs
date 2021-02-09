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

    public class ScatterSpecificItem : BaseEntityGenerationStrategy, IEntityGenerator
    {
        public string Item;

        private class ItemList: Dictionary<int, HashSet<IEntity>> { }

        public override void Generate(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branch, IRandom random)
        {

            var item = systemContainer.EntityEngine.GetEntityWithName(Item);

            foreach(var map in generatedBranch.Maps)
            {
                FillMap(map, systemContainer, item, random);
            }
        }

        private void FillMap(IMap map, ISystemContainer systemContainer, IEntity item, IRandom random)
        {
            var mapSize = map.Cells.Count;

            var numberOfItems = (int)Math.Ceiling(Density * mapSize);

            for (int i = 0; i< numberOfItems; i++)
            {
                SpawnMonster(map, systemContainer, item, random);
            }
        }

        private void SpawnMonster(IMap map, ISystemContainer systemContainer, IEntity item, IRandom random)
        {
            int retries = 25;

            MapCoordinate emptyLocation = null;

            for (int i = 0; i < retries; i++)
            {
                emptyLocation = map.GetQuickEmptyPosition(systemContainer.PositionSystem, random);

                if (emptyLocation != null) break;
            }

            if (emptyLocation == null || item == null) return;

            var created = systemContainer.PrototypeSystem.CreateAt(item, emptyLocation);
        }
    }
}

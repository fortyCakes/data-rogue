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
    public class RandomItemPlacement : BaseEntityGenerationStrategy, IEntityGenerator
    {

        public override void Generate(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branch, IRandom random, IProgress<string> progress)
        {
            progress.Report("Generating random items");

            var power = BasePower;

            var itemList = GetItemList(systemContainer, branch);

            IItemGenerator itemGenerator = GetItemGenerator(systemContainer, itemList);

            foreach (var map in generatedBranch.Maps)
            {
                FillMap(map, systemContainer, power, itemList, random, itemGenerator);

                power += PowerIncrement;
            }
        }

        protected virtual IItemGenerator GetItemGenerator(ISystemContainer systemContainer, List<IEntity> itemList)
        {
            return new RandomItemGenerator(systemContainer, itemList);
        }

        private List<IEntity> GetItemList(ISystemContainer systemContainer, IEntity branch)
        {
            return systemContainer.EntityEngine
                .AllEntities
                .Where(e => e.Has<Prototype>() && e.Has<Item>())
                .ToList();
        }

        private void FillMap(IMap map, ISystemContainer systemContainer, int power, List<IEntity> itemList, IRandom random, IItemGenerator itemGenerator)
        {
            var mapSize = map.Cells.Count;

            var numberOfItems = (int)Math.Ceiling(Density * mapSize);

            for (int i = 0; i< numberOfItems; i++)
            {
                SpawnItem(map, systemContainer, power, itemList, random, itemGenerator);
            }
        }

        private void SpawnItem(IMap map, ISystemContainer systemContainer, int power, List<IEntity> itemList, IRandom random, IItemGenerator itemGenerator)
        {
            int retries = 25;

            MapCoordinate emptyLocation = null;
            IEntity item = null;

            for (int i = 0; i < retries; i++)
            {
                emptyLocation = map.GetQuickEmptyPosition(systemContainer.PositionSystem, random);

                if (emptyLocation != null) break;
            }

            if (emptyLocation == null) return;

            for (int i = 0; i < retries; i++)
            {
                var itemLevel = random.Between(0, power + 2);
                item = itemGenerator.GenerateItem(itemList, itemLevel, random);

                if (item != null) break;
            }

            if (item == null) return;

            systemContainer.PositionSystem.SetPosition(item, emptyLocation);
        }
    }
}

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

    public class RandomItemGeneration : IEntityGenerator
    {
        private class ItemList: Dictionary<int, HashSet<IEntity>> { }

        public string GenerationType => "RandomItemGeneration";

        public void Generate(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branch, EntityGenerationStrategy step, IRandom random)
        {
            var power = step.BasePower;

            var monsterList = GetItemList(systemContainer, branch);

            foreach(var map in generatedBranch.Maps)
            {
                FillMap(map, systemContainer, step, power, monsterList, random);

                power += step.PowerIncrement;
            }
        }
        
        private ItemList GetItemList(ISystemContainer systemContainer, IEntity branch)
        {
            var list = new ItemList();

            var items = systemContainer.EntityEngine
                .AllEntities
                .Where(e => e.Has<Prototype>() && e.Has<Item>());

            foreach(var item in items)
            {
                var itemLevel = item.Get<Item>().ItemLevel;

                if (list.ContainsKey(itemLevel))
                {
                    list[itemLevel].Add(item);
                }
                else
                {
                    list.Add(itemLevel, new HashSet<IEntity> { item });
                }
            }

            return list;
        }

        private void FillMap(Map map, ISystemContainer systemContainer, EntityGenerationStrategy step, int power, ItemList itemList, IRandom random)
        {
            var mapSize = map.Cells.Count;

            var numberOfMonsters = (int)Math.Ceiling(step.Density * mapSize);

            for (int i = 0; i< numberOfMonsters; i++)
            {
                SpawnMonster(map, systemContainer, power, itemList, random);
            }
        }

        private void SpawnMonster(Map map, ISystemContainer systemContainer, int power, ItemList itemList, IRandom random)
        {
            int retries = 25;

            MapCoordinate emptyLocation = null;
            IEntity item = null;

            for (int i = 0; i < retries; i++)
            {
                emptyLocation = map.GetQuickEmptyPosition(systemContainer.PrototypeSystem, systemContainer.PositionSystem, random);

                if (emptyLocation != null) break;
            }

            for (int i = 0; i < retries; i++)
            {
                var randomPower = random.Between(0, power + 2);
                item = PickItem(itemList, randomPower, random);

                if (item != null) break;
            }

            if (emptyLocation == null || item == null) return;

            systemContainer.PrototypeSystem.CreateAt(item, emptyLocation);
        }

        private IEntity PickItem(ItemList itemList, int power, IRandom random)
        {
            if (!itemList.ContainsKey(power))
            {
                return null;
            }

            return random.PickOne(itemList[power].ToList());
        }
    }
}

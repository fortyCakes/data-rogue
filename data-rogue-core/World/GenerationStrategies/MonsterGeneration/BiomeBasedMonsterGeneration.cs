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

    public class BiomeBasedMonsterGeneration : IEntityGenerator
    {
        private class MonsterList: Dictionary<int, HashSet<IEntity>> { }

        public string GenerationType => "BiomeBasedMonsterGeneration";

        public void Generate(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branch, EntityGenerationStrategy step, IRandom random)
        {
            var power = step.BasePower;

            var monsterList = GetMonsterList(systemContainer, branch);

            foreach(var map in generatedBranch.Maps)
            {
                FillMap(map, systemContainer, step, power, monsterList, random);

                power += step.PowerIncrement;
            }
        }

        // This is probably hella slow, but it's the easy way to do this. Would be better to cache lists by biome in a System.
        private MonsterList GetMonsterList(ISystemContainer systemContainer, IEntity branch)
        {
            var list = new MonsterList();

            var monsters = systemContainer.EntityEngine
                .AllEntities
                .Where(e => e.HasAll(new SystemComponents { typeof(Biome), typeof(Fighter), typeof(Prototype)}))
                .Where(e => HasMatchingBiome(branch, e));

            foreach(var monster in monsters)
            {
                var cr = monster.Has<Challenge>() ? monster.Get<Challenge>().ChallengeRating : 0;

                if (list.ContainsKey(cr))
                {
                    list[cr].Add(monster);
                }
                else
                {
                    list.Add(cr, new HashSet<IEntity> { monster });
                }
            }

            return list;
        }

        private bool HasMatchingBiome(IEntity x, IEntity y)
        {
            if (!x.Has<Biome>() || !y.Has<Biome>())
            {
                return false;
            }

            return GetBiomes(x).Any(b => GetBiomes(y).Contains(b));
        }

        private static IEnumerable<string> GetBiomes(IEntity entity)
        {
            return entity.Components.OfType<Biome>().Select(b => b.Name);
        }

        private void FillMap(Map map, ISystemContainer systemContainer, EntityGenerationStrategy step, int power, MonsterList monsterList, IRandom random)
        {
            var mapSize = map.Cells.Count;

            var numberOfMonsters = (int)Math.Ceiling(step.Density * mapSize);

            for (int i = 0; i< numberOfMonsters; i++)
            {
                SpawnMonster(map, systemContainer, power, monsterList, random);
            }
        }

        private void SpawnMonster(Map map, ISystemContainer systemContainer, int power, MonsterList monsterList, IRandom random)
        {
            int retries = 25;

            MapCoordinate emptyLocation = null;
            IEntity monster = null;

            for (int i = 0; i < retries; i++)
            {
                emptyLocation = map.GetQuickEmptyPosition(systemContainer.PrototypeSystem, systemContainer.PositionSystem, random);

                if (emptyLocation != null) break;
            }

            for (int i = 0; i < retries; i++)
            {
                var randomPower = power + random.Between(-5, 3);
                monster = PickMonsterToSpawn(monsterList, randomPower, random);

                if (monster != null) break;
            }

            if (emptyLocation == null || monster == null) return;

            systemContainer.PrototypeSystem.CreateAt(monster, emptyLocation);
        }

        private IEntity PickMonsterToSpawn(MonsterList monsterList, int power, IRandom random)
        {
            if (!monsterList.ContainsKey(power))
            {
                return null;
            }

            return random.PickOne(monsterList[power].ToList());
        }
    }
}

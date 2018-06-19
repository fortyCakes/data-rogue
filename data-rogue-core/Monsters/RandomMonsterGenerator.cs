using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using data_rogue_core.Entities;
using data_rogue_core.Map;
using RogueSharp.Random;

namespace data_rogue_core.Monsters
{
    public class RandomMonsterGenerator : IMonsterGenerator
    {
        private readonly IRandom _random;
        public List<IMonsterFactory> MonsterFactories = new List<IMonsterFactory>();

        public RandomMonsterGenerator(IEnumerable<IMonsterFactory> monsterFactories, IRandom random)
        {
            _random = random;
            MonsterFactories = monsterFactories.ToList();
        }

        public Monster GetNewMonster()
        {
            return GetRandomMonsterFromMonsterFactoryList(MonsterFactories);
        }

        private Monster GetRandomMonsterFromMonsterFactoryList(List<IMonsterFactory> monsterFactories)
        {
            var monsterFactoriesCount = monsterFactories.Count;
            var index = _random.Next(monsterFactoriesCount - 1);

            var monsterFactory = monsterFactories[index];

            return monsterFactory.GetMonster();
        }

        public Monster GetNewMonsterWithTag(List<string> tags)
        {
            var taggedMonsterFactories = MonsterFactories.Where(mf => tags.All(tag => mf.Is(tag))).ToList();

            if (taggedMonsterFactories.Any())
            {
                return GetRandomMonsterFromMonsterFactoryList(taggedMonsterFactories);
            }

            return null;
        }
    }
}
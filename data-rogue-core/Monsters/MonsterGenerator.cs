using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using data_rogue_core.Data.Monsters;
using data_rogue_core.Entities;
using data_rogue_core.Map;

namespace data_rogue_core.Monsters
{
    public class RandomMonsterGenerator : IMonsterGenerator
    {
        public List<IMonsterFactory> MonsterFactories = new List<IMonsterFactory>();

        public RandomMonsterGenerator()
        {
            var parser = new MonsterFactoryDataParser();
            ResourceSet resourceSet = MonsterDataResources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);

            foreach (DictionaryEntry entry in resourceSet)
            {
                var json = entry.Value.ToString();
                MonsterFactories.Add(parser.GetMonsterFactory(json));
            }
        }

        public Monster GetNewMonster()
        {
            var monsterFactoriesCount = MonsterFactories.Count;
            var index = Game.Random.Next(monsterFactoriesCount-1);

            var monsterFactory = MonsterFactories[index];

            return monsterFactory.GetMonster();
        }
    }
}
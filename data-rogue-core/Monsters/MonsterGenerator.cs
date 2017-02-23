using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\Monsters\");

            foreach (string file in Directory.EnumerateFiles(path, "*.json", SearchOption.AllDirectories))
            {
                var json = File.ReadAllText(file);
                MonsterFactories.Add(parser.GetMonsterFactory(json));
            }
        }

        public Monster GetNewMonster()
        {
            return GetRandomMonsterFromMonsterFactoryList(MonsterFactories);
        }

        private Monster GetRandomMonsterFromMonsterFactoryList(List<IMonsterFactory> monsterFactories )
        {
            var monsterFactoriesCount = monsterFactories.Count;
            var index = Game.Random.Next(monsterFactoriesCount - 1);

            var monsterFactory = monsterFactories[index];

            return monsterFactory.GetMonster();
        }

        public Monster GetNewMonsterWithTag(string tag)
        {
            var taggedMonsterFactories = MonsterFactories.Where(mf => mf.Is(tag)).ToList();

            if (taggedMonsterFactories.Any())
            {
                return GetRandomMonsterFromMonsterFactoryList(taggedMonsterFactories);
            }

            return null;
        }
    }
}
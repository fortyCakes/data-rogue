using System.Collections.Generic;
using System.IO;
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
            var monsterFactoriesCount = MonsterFactories.Count;
            var index = Game.Random.Next(monsterFactoriesCount-1);

            var monsterFactory = MonsterFactories[index];

            return monsterFactory.GetMonster();
        }
    }
}
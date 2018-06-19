using System.Collections.Generic;
using System.IO;
using System.Reflection;
using data_rogue_core.Monsters;

namespace data_rogue_core
{
    public class MonsterDataLoader
    {
        public static IEnumerable<IMonsterFactory> GetMonsterData()
        {
            List<IMonsterFactory> monsterFactories = new List<IMonsterFactory>();

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\Monsters\");

            var monsterData = Directory.EnumerateFiles(path, "*.json", SearchOption.AllDirectories);

            var parser = new MonsterFactoryDataParser();

            foreach (string file in monsterData)
            {
                var json = File.ReadAllText(file);
                monsterFactories.Add(parser.GetMonsterFactory(json));
            }

            return monsterFactories;
        }
    }
}
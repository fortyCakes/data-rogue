using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Display;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RLNET;
using RogueSharp.DiceNotation;

namespace data_rogue_core.Monsters
{
    public class MonsterFactoryDataParser : IMonsterFactoryDataParser
    {
        public IMonsterFactory GetMonsterFactory(string monsterJson)
        {
            MonsterData monsterData = JsonConvert.DeserializeObject<MonsterData>(monsterJson);

            return new DefaultMonsterFactory(
                name: monsterData.Name,
                symbol: monsterData.Symbol,
                color: GetNamedColor(monsterData.Color), 
                attack: Dice.Parse(monsterData.Attack),
                attackChance: Dice.Parse(monsterData.AttackChance),
                defense: Dice.Parse(monsterData.Defense),
                defenseChance: Dice.Parse(monsterData.DefenseChance),
                gold: Dice.Parse(monsterData.Gold),
                health: Dice.Parse(monsterData.Health),
                speed: Dice.Parse(monsterData.Speed),
                awareness: Dice.Parse(monsterData.Awareness)
            );
        }

        private RLColor GetNamedColor(string colorName)
        {
            return Colors.GetColor(colorName);
        }
    }
    

    [JsonObject]
    public struct MonsterData
    {
        public string Attack;
        public string AttackChance;
        public string Awareness;
        public string Color;
        public string Defense;
        public string DefenseChance;
        public string Gold;
        public string Health;
        public string MaxHealth;
        public string Name;
        public string Speed;
        public char Symbol;
    }
}

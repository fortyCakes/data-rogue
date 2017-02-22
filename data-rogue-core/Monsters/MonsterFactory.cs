using data_rogue_core.Display;
using data_rogue_core.Entities;
using RLNET;
using RogueSharp.DiceNotation;

namespace data_rogue_core.Monsters
{
    public class DefaultMonsterFactory : IMonsterFactory
    {
        public DefaultMonsterFactory(
            string name,
            RLColor color,
            char symbol,
            DiceExpression attack,
            DiceExpression attackChance,
            DiceExpression awareness,
            DiceExpression defense,
            DiceExpression defenseChance,
            DiceExpression health,
            DiceExpression speed,
            DiceExpression gold
        )
        {
            Name = name;
            Color = color;
            Symbol = symbol;
            Attack = attack;
            AttackChance = attackChance;
            Awareness = awareness;
            Defense = defense;
            DefenseChance = defenseChance;
            Health = health;
            Speed = speed;
            Gold = gold;
        }

        public char Symbol { get; private set; }
        public DiceExpression Attack { get; private set; }
        public DiceExpression AttackChance { get; private set; }
        public DiceExpression Awareness { get; private set; }
        public DiceExpression Defense { get; private set; }
        public DiceExpression DefenseChance { get; private set; }
        public DiceExpression Health { get; private set; }
        public DiceExpression Speed { get; private set; }
        public DiceExpression Gold { get; private set; }

        public RLColor Color { get; set; }

        public string Name { get; set; }

        public Monster GetMonster()
        {
            int health = Health.Roll().Value;

            return new Monster()
            {
                Attack = Attack.Roll().Value,
                AttackChance = AttackChance.Roll().Value,
                Awareness = Awareness.Roll().Value,
                Color = Color,
                Defense = Defense.Roll().Value,
                DefenseChance = DefenseChance.Roll().Value,
                Gold=Gold.Roll().Value,
                Health = health,
                MaxHealth = health,
                Name=Name,
                Speed = Speed.Roll().Value,
                Symbol = Symbol,
                TurnsAlerted = null
            };
        }
    }
}
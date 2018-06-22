using System;
using System.Collections.Generic;
using System.Linq;
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
            DiceExpression gold,
            List<string> tags
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
            Tags = tags;
        }

        public List<string> Tags { get;}

        public bool Is(string tag)
        {
            return Tags.Any(t => string.Equals(t, tag, StringComparison.InvariantCultureIgnoreCase));
        }

        public char Symbol { get; }
        public DiceExpression Attack { get; }
        public DiceExpression AttackChance { get; }
        public DiceExpression Awareness { get; }
        public DiceExpression Defense { get; }
        public DiceExpression DefenseChance { get; }
        public DiceExpression Health { get; }
        public DiceExpression Speed { get; }
        public DiceExpression Gold { get; }

        public RLColor Color { get; set; }

        public string Name { get; set; }

        public Monster GetMonster()
        {
            int health = Health.Roll().Value;

            return new Monster()
            {
                Name = Name,
                Attack = Attack.Roll().Value,
                AttackChance = AttackChance.Roll().Value,
                Awareness = Awareness.Roll().Value,
                Color = Color,
                Defense = Defense.Roll().Value,
                DefenseChance = DefenseChance.Roll().Value,
                Gold=Gold.Roll().Value,
                HealthCounter = new HealthCounter(health),
                AuraCounter = new AuraCounter(0),
                Speed = Speed.Roll().Value,
                Symbol = Symbol,
                Tags = Tags,
                TurnsAlerted = null
            };
        }
    }
}
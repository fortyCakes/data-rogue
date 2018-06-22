using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Display;
using data_rogue_core.Interfaces;
using RLNET;
using RogueSharp;

namespace data_rogue_core.Entities
{
    public class Actor : IActor, IDrawable, IScheduleable, ITaggable
    {

        // ITaggable
        public List<string> Tags { get; set; } = new List<string>();

        public bool Is(string tag)
        {
            return Tags.Any(t => string.Equals(t, tag, StringComparison.InvariantCultureIgnoreCase));
        }

        // IActor
        public string Name { get; set; }
        public int Awareness { get; set; }

        // IDrawable
        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        // Other
        public int Attack { get; set; }

        public int Speed { get; set; }

        public HealthCounter HealthCounter { get; set; }

        public AuraCounter AuraCounter { get; set; }

        public int MaxHealth => HealthCounter.CounterMax;
        public int CurrentHealth => HealthCounter.CounterValue;

        public int Gold { get; set; }

        public int DefenseChance { get; set; }

        public int Defense { get; set; }

        public int AttackChance { get; set; }

        public int Time => Speed;
        
        public void Draw(IRLConsoleWriter console, IMap map, int xOffset, int yOffset)
        {
            // Don't draw actors in cells that haven't been explored
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            // Only draw the actor with the color and symbol when they are in field-of-view
            if (map.IsInFov(X, Y))
            {
                console.Set(X-xOffset, Y-yOffset, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                // When not in field-of-view just draw a normal floor
                console.Set(X-xOffset, Y-yOffset, Colors.Floor, Colors.FloorBackground, '.');
            }
        }


        public void TakeDamage(int damage)
        {
            HealthCounter.TakeDamage(damage);
        }

        public void Heal(int healing, bool canOverheal)
        {
            HealthCounter.Restore(healing, canOverheal);
        }

        public void TakeAuraDamage(int damage)
        {
            AuraCounter.TakeDamage(damage);
        }

        public void RestoreAura(int restoration)
        {
            AuraCounter.Restore(restoration);
        }
    }
}
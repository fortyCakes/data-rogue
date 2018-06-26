using System;
using data_rogue_core.Display;
using data_rogue_core.Entities;
using data_rogue_core.Interfaces;
using RLNET;
using RogueSharp;

namespace data_rogue_core.Core
{
    public class Ability : IAbility, IDrawable
    {
        public Ability()
        {
            Symbol = '*';
            Color = RLColor.Yellow;
        }

        public string Name { get; protected set; }

        public int TurnsToRefresh { get; protected set; }

        public int TurnsUntilRefreshed { get; protected set; }

        public bool Perform()
        {
            if (TurnsUntilRefreshed > 0)
            {
                Game.MessageLog.Add($"{Name} isn't refreshed yet ({TurnsUntilRefreshed}/{TurnsToRefresh})");
                return false;
            }

            TurnsUntilRefreshed = TurnsToRefresh;

            return PerformAbility();
        }

        protected virtual bool PerformAbility()
        {
            return false;
        }


        public void Tick()
        {
            if (TurnsUntilRefreshed > 0)
            {
                TurnsUntilRefreshed--;
            }
        }

        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void Draw(IRLConsoleWriter console, IMap map, int atX, int atY)
        {
            throw new NotImplementedException();
        }
    }
}

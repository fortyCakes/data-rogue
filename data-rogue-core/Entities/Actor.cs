using data_rogue_core.Display;
using data_rogue_core.Interfaces;
using RLNET;
using RogueSharp;

namespace data_rogue_core.Entities
{
    public class Actor : IActor, IDrawable, IScheduleable
    {
        private int mAttack;
        private int mSpeed;
        private int mMaxHealth;
        private int mHealth;
        private int mGold;
        private int mDefenseChance;
        private int mDefense;
        private int mAttackChance;
        // IActor
        public string Name { get; set; }
        public int Awareness { get; set; }

        // IDrawable
        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        // Other
        public int Attack
        {
            get { return mAttack; }
            set { mAttack = value; }
        }

        public int Speed
        {
            get { return mSpeed; }
            set { mSpeed = value; }
        }

        public int MaxHealth
        {
            get { return mMaxHealth; }
            set { mMaxHealth = value; }
        }

        public int Health
        {
            get { return mHealth; }
            set { mHealth = value; }
        }

        public int Gold
        {
            get { return mGold; }
            set { mGold = value; }
        }

        public int DefenseChance
        {
            get { return mDefenseChance; }
            set { mDefenseChance = value; }
        }

        public int Defense
        {
            get { return mDefense; }
            set { mDefense = value; }
        }

        public int AttackChance
        {
            get { return mAttackChance; }
            set { mAttackChance = value; }
        }

        public int Time
        {
            get
            {
                return Speed;
            }
        }

        public void Draw(RLConsole console, IMap map)
        {
            // Don't draw actors in cells that haven't been explored
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            // Only draw the actor with the color and symbol when they are in field-of-view
            if (map.IsInFov(X, Y))
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                // When not in field-of-view just draw a normal floor
                console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
            }
        }

        
    }
}
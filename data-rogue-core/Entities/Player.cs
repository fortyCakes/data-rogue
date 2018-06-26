using data_rogue_core.Abilities;
using data_rogue_core.Core;
using data_rogue_core.Display;
using RLNET;

namespace data_rogue_core.Entities
{
    public class Player : Actor
    {
        public Player()
        {
            Attack = 2;
            AttackChance = 50;
            Awareness = 15;
            Color = Colors.Player;
            Defense = 2;
            DefenseChance = 40;
            Gold = 0;
            HealthCounter = new HealthCounter(100);
            AuraCounter = new AuraCounter(0);
            Name = "Rogue";
            Speed = 10;
            Symbol = '@';
            Ability1 = new DecisiveAttackAbility();
            Ability2 = new DoNothing();
            Ability3 = new DoNothing();
            Ability4 = new DoNothing();
        }

        public Ability Ability1 { get; set; }
        public Ability Ability2 { get; set; }
        public Ability Ability3 { get; set; }
        public Ability Ability4 { get; set; }
        public int CurrentAura => AuraCounter.CurrentAura;

        public void DrawStats(RLConsole statConsole)
        {
            statConsole.Print(1, 1, $"Name:    {Name}", Colors.Text);
            statConsole.Print(1, 3, $"Health:  {HealthCounter}", Colors.Text);
            statConsole.Print(1, 5, $"Attack:  {Attack} ({AttackChance}%)", Colors.Text);
            statConsole.Print(1, 7, $"Defense: {Defense} ({DefenseChance}%)", Colors.Text);
            statConsole.Print(1, 9, $"Gold:    {Gold}", Colors.Gold);

            statConsole.Print(1, 11, $"Abilities:", Colors.Text);
            statConsole.Print(1, 12, $"  1.{Ability1.Name}", Ability1.Color);
            statConsole.Print(1, 13, $"  2.{Ability2.Name}", Ability2.Color);
            statConsole.Print(1, 14, $"  3.{Ability3.Name}", Ability3.Color);
            statConsole.Print(1, 15, $"  4.{Ability4.Name}", Ability4.Color);

            statConsole.Print(1, 17, $"Aura:   {AuraCounter}", Colors.Text);
        }

        public override void Tick()
        {
            base.Tick();

            Ability1.Tick();
            Ability2.Tick();
            Ability3.Tick();
            Ability4.Tick();
        }
    }
}
using System;
using data_rogue_core.Core;
using data_rogue_core.Display;
using data_rogue_core.Entities;
using data_rogue_core.Interfaces;
using data_rogue_core.Map;
using RogueSharp;

namespace data_rogue_core.Abilities
{
    public class DecisiveAttackAbility : Ability, ITargetable
    {
        public DecisiveAttackAbility()
        {
            Name = "Decisive Attack";
            TurnsToRefresh = 2;
            TurnsUntilRefreshed = 0;
            Color = Colors.Text;
            Symbol = 'D';
        }

        protected override bool PerformAbility()
        {
            Player player = Game.Player;
            if (player.CurrentAura <= 10)
            {
                Game.MessageLog.Add("You need at least 10 Aura to make a Decisive Attack.");
                return false;
            }
            else
            {
                return Game.TargetingSystem.SelectMonster(this);
            }
        }

        public void SelectTarget(Point target)
        {
            DungeonMap map = Game.DungeonMap;
            Player player = Game.Player;
            Monster monster = map.GetMonsterAt(target.X, target.Y);
            if (monster != null)
            {
                Game.MessageLog.Add($"{player.Name} unleashes a {this.Name} at {monster.Name}");
                int aura = player.AuraCounter.CurrentAura;
                Actor decisiveAttackActor = new Actor
                {
                    Attack = aura,
                    AttackChance = 50,
                    Name = this.Name
                };
                Game.CommandSystem.DecisiveAttack(decisiveAttackActor, monster);

                player.AuraCounter.SetTo(0);
            }
        }
    }
}

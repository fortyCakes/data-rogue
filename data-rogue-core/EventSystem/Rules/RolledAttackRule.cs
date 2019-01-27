using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.EventSystem.Rules
{
    class RolledAttackRule : IEventRule
    {
        private readonly IRandom random;

        public RolledAttackRule(ISystemContainer systemContainer)
        {
            random = systemContainer.Random;
            EntityEngine = systemContainer.EntityEngine;
            EventRuleSystem = systemContainer.EventSystem;
            MessageSystem = systemContainer.MessageSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Attack };
        public int RuleOrder => 0;

        private IEntityEngine EntityEngine { get; }
        public IEventSystem EventRuleSystem { get; }
        public IMessageSystem MessageSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var defender = (IEntity) eventData;

            var attackerFighter = sender.Get<Fighter>();
            var defenderFighter = defender.Get<Fighter>();

            var attackerDescription = sender.Get<Description>();
            var defenderDescription = defender.Get<Description>();

            var attackStat = EventRuleSystem.GetStat(sender, Stat.Agility);
            var defenceStat = EventRuleSystem.GetStat(defender, Stat.Agility);

            var attackRoll = random.StatCheck(attackStat);
            var defenceRoll = random.StatCheck(defenceStat);

            bool hit = attackRoll >= defenceRoll;

            MessageSystem.Write($"{attackerDescription.Name} attacks {defenderDescription.Name}, rolls {attackRoll}/{attackStat} vs {defenceRoll}/{defenceStat}. {(hit ? "Hit" : "Miss")}", Color.White);
            
            return hit;
        }
    }
}

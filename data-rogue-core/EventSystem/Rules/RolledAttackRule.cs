using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.EventSystem.Rules
{
    class RolledAttackRule : IEventRule
    {
        private readonly IRandom _random;

        public RolledAttackRule(IEntityEngine engine, IRandom random, IEventRuleSystem eventRuleSystem, IMessageSystem messageSystem)
        {
            _random = random;
            EntityEngine = engine;
            EventRuleSystem = eventRuleSystem;
            MessageSystem = messageSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Attack };
        public int RuleOrder => 0;

        private IEntityEngine EntityEngine { get; }
        public IEventRuleSystem EventRuleSystem { get; }
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

            var attackRoll = _random.StatCheck(attackStat);
            var defenceRoll = _random.StatCheck(defenceStat);

            bool hit = attackRoll >= defenceRoll;

            MessageSystem.Write($"{attackerDescription.Name} attacks {defenderDescription.Name}, rolls {attackRoll}/{attackStat} vs {defenceRoll}/{defenceStat}. {(hit ? "Hit" : "Miss")}", Color.White);
            
            return hit;
        }
    }
}

using System;
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
            var data = eventData as AttackEventData;

            var defender = data.Defender;

            var attackerFighter = sender.Get<Fighter>();
            var defenderFighter = defender.Get<Fighter>();

            var attackerDescription = sender.Get<Description>();
            var defenderDescription = defender.Get<Description>();

            var attackStat = GetAttackStat(sender, data.AttackType);
            var defenceStat = GetDefenceStat(sender, data.AttackType);

            var attackRoll = Math.Max(random.StatCheck(attackStat), random.StatCheck(attackStat));
            var defenceRoll = random.StatCheck(defenceStat);

            bool hit = attackRoll >= defenceRoll;
            
            return hit;
        }

        private decimal GetAttackStat(IEntity entity, AttackType attackType)
        {
            switch(attackType)
            {
                case AttackType.Physical:
                    return EventRuleSystem.GetStat(entity, Stat.Agility);
                case AttackType.Magical:
                    return EventRuleSystem.GetStat(entity, Stat.Intellect);
                default:
                    throw new NotImplementedException();
            }
        }

        private decimal GetDefenceStat(IEntity entity, AttackType attackType)
        {
            switch (attackType)
            {
                case AttackType.Physical:
                    return EventRuleSystem.GetStat(entity, Stat.Agility);
                case AttackType.Magical:
                    return EventRuleSystem.GetStat(entity, Stat.Agility);
                default:
                    throw new NotImplementedException();
            }
        }


    }
}

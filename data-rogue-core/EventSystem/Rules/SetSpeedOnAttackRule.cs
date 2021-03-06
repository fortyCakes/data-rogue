﻿using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class SetSpeedOnAttackRule : IEventRule
    {
        private ISystemContainer _systemContainer;

        public SetSpeedOnAttackRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public uint RuleOrder => 998;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            if (data.Speed == null)
            {
                data.Speed = (ulong)_systemContainer.StatSystem.GetEntityStat(data.Weapon, "Speed");
            }

            return true;
        }
    }
}

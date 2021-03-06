﻿using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class DoXpGainRule : IEventRule
    {
        public EventTypeList EventTypes => new EventTypeList { EventType.GainXP };
        public EventRuleType RuleType => EventRuleType.EventResolution;

        public uint RuleOrder => 0;

        public DoXpGainRule(ISystemContainer systemContainer)
        {

        }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as GainXPEventData;

            var experience = sender.Get<Experience>();

            if (experience == null)
            {
                return false;
            }

            experience.Amount += data.Amount;

            return true;
        }
    }
}

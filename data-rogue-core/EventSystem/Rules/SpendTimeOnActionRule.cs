﻿using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class SpendTimeOnActionRule: IEventRule
    {
        private ISystemContainer systemContainer;

        public SpendTimeOnActionRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Action };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as ActionEventData;

            if (data.Action == ActionType.None || !data.IsAction) return true;

            if (data.Speed.Value > 0)
            {
                systemContainer.EventSystem.Try(EventType.SpendTime, sender, new SpendTimeEventData { Ticks = data.Speed.Value });
            }

            return true;
        }
    }

}

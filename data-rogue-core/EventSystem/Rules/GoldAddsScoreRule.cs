﻿using System;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class GoldAddsScoreRule : IEventRule
    {
        private readonly IItemSystem _itemSystem;

        public GoldAddsScoreRule(ISystemContainer systemContainer)
        {
            _itemSystem = systemContainer.ItemSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.GetStat };
        public uint RuleOrder => 0;

        public EventRuleType RuleType => EventRuleType.EventResolution;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = (GetStatEventData)eventData;

            switch (data.Stat)
            {
                case "Score":
                    data.Value += _itemSystem.CheckWealth(sender, "Gold");
                    break;
            }

            return true;
        }
    }
}

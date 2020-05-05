﻿using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class WaitAction : ApplyActionRule
    {
        public WaitAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.Wait;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var waitTime = int.Parse(eventData.Parameters);

            if (_systemContainer.EventSystem.Try(EventType.SpendTime, sender, new SpendTimeEventData {Ticks = (ulong)waitTime * sender.Get<Actor>().Speed}))
            {
                eventData.IsAction = true;
            }

            return true;
        }
    }
}
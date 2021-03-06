﻿using data_rogue_core.Activities;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_core.EventSystem.Rules
{

    public class MoveAction : ApplyActionRule
    {
        public MoveAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.Move;
        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var vector = Vector.Parse(eventData.Parameters);

            if (_systemContainer.EventSystem.Try(EventType.Move, sender, vector))
            {
                eventData.IsAction = true;
            }

            return true;
        }
    }
}

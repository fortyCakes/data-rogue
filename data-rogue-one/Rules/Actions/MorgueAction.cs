﻿using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_one.EventSystem.Utils;

namespace data_rogue_core.EventSystem.Rules
{

    public class MorgueAction : ApplyActionRule
    {
        public MorgueAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.Morgue;

        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            _systemContainer.SaveSystem.SaveMorgueFile(MorgueHelper.GenerateMorgueText(_systemContainer));
            
            return false;
        }
    }
}

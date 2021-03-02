using data_rogue_core.Activities;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_core.EventSystem.Rules
{

    public class EscapeMenuAction: ApplyActionRule
    {
        public EscapeMenuAction(ISystemContainer systemContainer) :  base(systemContainer)
        {

        }

        public override ActionType actionType => ActionType.EscapeMenu;
        public override ActivityType activityType => ActivityType.Menu;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            if (_systemContainer.ActivitySystem.Count > 1)
            {
                _systemContainer.ActivitySystem.RemoveActivity(_systemContainer.ActivitySystem.Peek());
            }

            return true;
        }
    }
}

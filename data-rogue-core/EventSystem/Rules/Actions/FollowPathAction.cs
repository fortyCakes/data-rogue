using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.EventSystem.Rules
{

    public class FollowPathAction : ApplyActionRule
    {
        public FollowPathAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }


        public override ActionType actionType => ActionType.FollowPath;
        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            if (!sender.Has<FollowPathBehaviour>())
            {
                var coordinates = eventData.Parameters.Split(';').Select(e =>
                {
                    var mapCoordinate = new MapCoordinate();
                    mapCoordinate.Deserialize(e);
                    return mapCoordinate;
                }).ToList();

                var behaviour = new FollowPathBehaviour(_systemContainer) { Path = coordinates, Priority = 999 };

                _systemContainer.EntityEngine.AddComponent(sender, behaviour);

                var firstStep = behaviour.ChooseAction(sender);

                if (firstStep != null)
                {
                    _systemContainer.EventSystem.Try(EventType.Action, sender, firstStep);
                }
            }

            return true;
        }
    }
}

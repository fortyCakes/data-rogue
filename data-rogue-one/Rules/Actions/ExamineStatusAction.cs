using System;
using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.EventSystem.Rules;
using data_rogue_core.Menus.DynamicMenus;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_one.EventSystem.Utils;

namespace data_rogue_one.EventSystem.Rules
{
    public class ExamineStatusAction: ApplyActionRule
    {
        public ExamineStatusAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.Examine;

        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var entityId = uint.Parse(eventData.Parameters);
            var entity = _systemContainer.EntityEngine.Get(entityId);

            _systemContainer.ActivitySystem.Push(new InformationActivity(_systemContainer.ActivitySystem, StatusHelper.GetStatusConfigurations(entity), entity, true, false));

            return false;
        }
    }

    public class ExamineStatusFromMenuAction : ApplyActionRule
    {
        public ExamineStatusFromMenuAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.Examine;

        public override ActivityType activityType => ActivityType.Menu;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var entityId = uint.Parse(eventData.Parameters);
            var entity = _systemContainer.EntityEngine.Get(entityId);

            _systemContainer.ActivitySystem.Push(new InformationActivity(_systemContainer.ActivitySystem, StatusHelper.GetStatusConfigurations(entity), entity, true, false));

            return false;
        }
    }

    public class PlayerStatusAction : ApplyActionRule
    {
        public PlayerStatusAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.PlayerStatus;

        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var player = _systemContainer.PlayerSystem.Player;

            _systemContainer.ActivitySystem.Push(new InformationActivity(_systemContainer.ActivitySystem, StatusHelper.GetStatusConfigurations(player), player, true, true));

            return false;
        }
    }
}
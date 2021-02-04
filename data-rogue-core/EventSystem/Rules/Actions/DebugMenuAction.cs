using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Menus.DynamicMenus;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class DebugMenuAction : ApplyActionRule
    {
        public DebugMenuAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.DebugMenu;
        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            _systemContainer.ActivitySystem.Push(new MenuActivity(new DebugMenu(_systemContainer)));

            return false;
        }
    }
}

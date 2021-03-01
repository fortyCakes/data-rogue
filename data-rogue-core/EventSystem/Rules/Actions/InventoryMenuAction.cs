using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Menus.DynamicMenus;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class InventoryMenuAction : ApplyActionRule
    {
        public InventoryMenuAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.InventoryMenu;
        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var inventory = sender.Get<Inventory>();

            _systemContainer.ActivitySystem.Push(new MenuActivity(
                _systemContainer.ActivitySystem.DefaultPosition,
                _systemContainer.ActivitySystem.DefaultPadding,
                new InventoryMenu(_systemContainer, inventory)));

            return false;
        }
    }
}

using data_rogue_core.Activities;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_core.EventSystem.Rules
{

    public class EscapeGameplayAction : ApplyActionRule
    {
        public EscapeGameplayAction(ISystemContainer systemContainer) : base(systemContainer)
        {

        }

        public override ActionType actionType => ActionType.EscapeMenu;
        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var confirmDialog = new ConfirmActivity(_systemContainer, "Are you sure you want to quit? Unsaved progress will be lost.", CloseGameplayActivity);
            _systemContainer.ActivitySystem.ActivityStack.Push(confirmDialog);

            return true;
        }

        private void CloseGameplayActivity()
        {
            _systemContainer.ActivitySystem.GameplayActivity.Running = false;
            _systemContainer.ActivitySystem.Push(new MenuActivity(new MainMenu(_systemContainer)));
        }
    }
}

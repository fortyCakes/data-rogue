using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class SaveAction : ApplyActionRule
    {
        public SaveAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.Save;
        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            _systemContainer.SaveSystem.Save();

            return false;
        }
    }
}

using data_rogue_core.Activities;
using data_rogue_core.Behaviours;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class RestAction : ApplyActionRule
    {
        public RestAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.Rest;

        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var restBehaviour = sender.Get<PlayerRestBehaviour>();

            restBehaviour.Resting = true;

            return true;
        }
    }
}

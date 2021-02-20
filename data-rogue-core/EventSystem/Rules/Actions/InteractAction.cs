using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class InteractAction: ApplyActionRule
    {
        public InteractAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.Interact;
        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var interaction = _systemContainer.InteractableSystem.GetCurrentInteractionFor(sender);

            if (interaction.Item1 != null)
            {
                _systemContainer.ScriptExecutor.ExecuteByName(interaction.Item1, interaction.Item2.Script, sender);
            }

            return false;
        }
    }
}

using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Behaviours
{
    public class PlayerControlledBehaviour : BaseBehaviour
    {
        public PlayerControlledBehaviour(ISystemContainer systemContainer)
        {

        }

        public override ActionEventData ChooseAction(IEntity entity)
        {
            return new ActionEventData {Action = ActionType.WaitForInput};
        }
    }
}
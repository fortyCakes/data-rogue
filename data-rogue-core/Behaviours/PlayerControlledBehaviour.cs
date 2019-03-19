using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;

namespace data_rogue_core.Behaviours
{
    public class PlayerControlledBehaviour : BaseBehaviour
    {
        public override ActionEventData ChooseAction(IEntity entity)
        {
            return new ActionEventData {Action = ActionType.WaitForInput};
        }
    }
}
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class MoveAction : ApplyActionRule
    {
        public MoveAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.Move;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var vector = Vector.Parse(eventData.Parameters);

            return _systemContainer.EventSystem.Try(EventType.Move, sender, vector);
        }
    }
}

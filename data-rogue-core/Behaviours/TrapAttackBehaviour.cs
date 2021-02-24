using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Behaviours
{
    public class TrapAttackBehaviour : BaseBehaviour
    {
        private readonly ISystemContainer _systemContainer;

        public TrapAttackBehaviour(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public override ActionEventData ChooseAction(IEntity entity)
        {
            var position = _systemContainer.PositionSystem.CoordinateOf(entity);

            var otherEntities = _systemContainer.PositionSystem.EntitiesAt(position);
            otherEntities.Remove(entity);

            if (otherEntities.Any(e => e.Has<Health>()))
            {
                var target = otherEntities.First();

                return new ActionEventData { Action = ActionType.MeleeAttack, Parameters = $"{entity.EntityId},{target.EntityId}", Speed = null, KeyPress = null };
            }

            return null;
        }
    }
}
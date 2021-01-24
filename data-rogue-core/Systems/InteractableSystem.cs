using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Systems
{
    public class InteractableSystem : BaseSystem, IInteractableSystem
    {
        private IPositionSystem _positionSystem;

        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Interactable), typeof(Position)};

        public override SystemComponents ForbiddenComponents => new SystemComponents();

        public InteractableSystem(IPositionSystem positionSystem)
        {
            _positionSystem = positionSystem;
        }

        public (IEntity, Interactable) GetCurrentInteractionFor(IEntity player)
        {
            // TODO interaction switching
            var entity = GetInteractablesNear(player).FirstOrDefault();
            if (entity == null) return (null, null);

            var interactable = entity.Components.OfType<Interactable>().First();

            return (entity, interactable);
        }

        public List<IEntity> GetInteractablesNear(IEntity entity)
        {
            var playerPosition = _positionSystem.CoordinateOf(entity);

            var nearbyPositions = TargetingSystem.GetAdjacentAndDiagonalCellVectors().Select(v => playerPosition + v);

            var interactables = Entities.Where(e => nearbyPositions.Contains(_positionSystem.CoordinateOf(e))).ToList();

            return interactables;
        }
    }
}

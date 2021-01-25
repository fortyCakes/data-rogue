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
    public class InteractionSystem : BaseSystem, IInteractionSystem
    {
        private IPositionSystem _positionSystem;

        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Interaction), typeof(Position)};

        public override SystemComponents ForbiddenComponents => new SystemComponents();

        public Dictionary<IEntity, Interaction> CurrentInteractions = new Dictionary<IEntity, Interaction>();

        public InteractionSystem(IPositionSystem positionSystem)
        {
            _positionSystem = positionSystem;
        }

        public override void Initialise()
        {
            base.Initialise();
            CurrentInteractions = new Dictionary<IEntity, Interaction>();
        }

        public (IEntity, Interaction) GetCurrentInteractionFor(IEntity entity)
        {
            var interactableEntities = GetInteractablesNear(entity);

            if (!interactableEntities.Any())
            {
                CurrentInteractions.Remove(entity);
                return (null, null);
            }
            IEnumerable<Interaction> interactables = PossibleInteractionsWith(interactableEntities);

            if (CurrentInteractions.Keys.Contains(entity))
            {
                var currentInteraction = CurrentInteractions[entity];

                if (interactables.Contains(currentInteraction))
                {
                    var interactWith = interactableEntities.Single(e => e.Components.Contains(currentInteraction));
                    return (interactWith, currentInteraction);
                }
                else
                {
                    CurrentInteractions.Remove(entity);
                }
            }

            var interactableEntity = interactableEntities.First();
            var interactable = interactableEntity.Components.OfType<Interaction>().First();

            CurrentInteractions[entity] = interactable;

            return (interactableEntity, interactable);
        }

        public List<IEntity> GetInteractablesNear(IEntity entity)
        {
            var playerPosition = _positionSystem.CoordinateOf(entity);

            var nearbyPositions = TargetingSystem.GetAdjacentAndDiagonalCellVectors().Select(v => playerPosition + v);

            var interactables = Entities.Where(e => nearbyPositions.Contains(_positionSystem.CoordinateOf(e))).ToList();

            return interactables;
        }

        public void NextInteraction(IEntity entity)
        {
            var interactableEntities = GetInteractablesNear(entity);
            var possibleInteractions = PossibleInteractionsWith(interactableEntities).ToList();

            var currentInteraction = GetCurrentInteractionFor(entity);

            if (currentInteraction.Item1 == null) return;

            var index = possibleInteractions.IndexOf(currentInteraction.Item2);

            index++;

            if (index >= possibleInteractions.Count())
            {
                index = 0;
            }

            CurrentInteractions[entity] = possibleInteractions[index];
        }

        private static IEnumerable<Interaction> PossibleInteractionsWith(List<IEntity> interactableEntities)
        {
            return interactableEntities.SelectMany(e => e.Components.OfType<Interaction>());
        }
    }
}

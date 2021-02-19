using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class BumpInteractRule : IEventRule
    {
        private ISystemContainer _systemContainer;

        public BumpInteractRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Move };
        public uint RuleOrder => 2;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;


        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (_systemContainer.PlayerSystem.IsPlayer(sender)) // Only the player can interact with things. Is this right?
            {
                var vector = (Vector)eventData;
                var targetCoordinate = _systemContainer.PositionSystem.CoordinateOf(sender) + vector;

                var entitiesAtPosition = _systemContainer.PositionSystem.EntitiesAt(targetCoordinate);

                if (entitiesAtPosition.Any(IsInteractable))
                {
                    if (sender.Has<FollowPathBehaviour>())
                    {
                        _systemContainer.EntityEngine.RemoveComponent(sender, sender.Get<FollowPathBehaviour>());
                    }

                    var interactWith = entitiesAtPosition.First(e => IsInteractable(e));

                    Interaction interaction = interactWith.Components.OfType<Interaction>().First();

                    _systemContainer.ScriptExecutor.ExecuteByName(interactWith, interaction.Script, sender);

                    return false;
                }
            }

            return true;
        }

        private bool IsInteractable(IEntity entity)
        {
            return entity.Has<Interaction>() && entity.Has<Physical>() && !entity.Get<Physical>().Passable;
        }
    }
}

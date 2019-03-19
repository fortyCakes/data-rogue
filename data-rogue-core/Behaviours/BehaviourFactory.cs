using System;
using System.Collections.Generic;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Behaviours
{
    public class BehaviourFactory : IBehaviourFactory
    {
        private IPositionSystem _positionSystem;
        private IEventSystem _eventRuleSystem;
        private IRandom _random;
        private IMessageSystem _messageSystem;
        private IPlayerSystem _playerSystem;
        private IMapSystem _mapSystem;

        private Dictionary<Type, Func<IBehaviour>> constructors;

        public BehaviourFactory(IPositionSystem positionSystem, IEventSystem eventRuleSystem, IRandom random, IMessageSystem messageSystem, IPlayerSystem playerSystem, IMapSystem mapSystem)
        {
            _positionSystem = positionSystem;
            _eventRuleSystem = eventRuleSystem;
            _random = random;
            _messageSystem = messageSystem;
            _playerSystem = playerSystem;
            _mapSystem = mapSystem;

            constructors = new Dictionary<Type, Func<IBehaviour>>()
            {
                { typeof(PlayerControlledBehaviour), () => new PlayerControlledBehaviour() },
                { typeof(MoveToPlayerBehaviour), () => new MoveToPlayerBehaviour(_positionSystem, _eventRuleSystem, _playerSystem, _mapSystem) },
                { typeof(RandomlyMoveBehaviour), () => new RandomlyMoveBehaviour(_positionSystem, _eventRuleSystem, _random) },
                { typeof(PlayerRestBehaviour), () => new PlayerRestBehaviour(_eventRuleSystem, _messageSystem) },
            };
        }

        public IBehaviour Get(Type behaviourType)
        {
            if (!constructors.ContainsKey(behaviourType))
            {
                throw new Exception("Behaviour type not found in BehaviourFactory.");
            }

            return constructors[behaviourType]();
            
        }
    }
}
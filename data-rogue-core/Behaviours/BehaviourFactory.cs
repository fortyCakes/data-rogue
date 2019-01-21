﻿using System;
using System.Collections.Generic;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems;

namespace data_rogue_core.Behaviours
{
    public class BehaviourFactory : IBehaviourFactory
    {
        private IPositionSystem _positionSystem;
        private IEventSystem _eventRuleSystem;
        private IRandom _random;


        private Dictionary<Type, Func<IBehaviour>> constructors;
        

        public BehaviourFactory(IPositionSystem positionSystem, IEventSystem eventRuleSystem, IRandom random)
        {
            _positionSystem = positionSystem;
            _eventRuleSystem = eventRuleSystem;
            _random = random;
            
            constructors = new Dictionary<Type, Func<IBehaviour>>()
            {
                { typeof(PlayerControlledBehaviour), () => new PlayerControlledBehaviour() },
                { typeof(MoveToPlayerBehaviour), () => new MoveToPlayerBehaviour(_positionSystem, _eventRuleSystem) },
                { typeof(RandomlyMoveBehaviour), () => new RandomlyMoveBehaviour(_positionSystem, _eventRuleSystem, _random) },
                
                { typeof(TestBehaviour), () => new TestBehaviour(_positionSystem, _eventRuleSystem) },
            };
        }

        public IBehaviour Get(Type type)
        {
            if (!constructors.ContainsKey(type))
            {
                throw new Exception("Behaviour type not found in BehaviourFactory.");
            }

            return constructors[type]();
            
        }
    }
}
using System;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems;

namespace data_rogue_core.Behaviours
{
    public class BehaviourFactory : IBehaviourFactory
    {
        private IPositionSystem _positionSystem;
        private IEventRuleSystem _eventRuleSystem;
        private IRandom _random;

        private PlayerControlledBehaviour playerControlledBehaviour;
        private RandomlyMoveBehaviour randomlyMoveBehaviour;

        public BehaviourFactory(IPositionSystem positionSystem, IEventRuleSystem eventRuleSystem, IRandom random)
        {
            _positionSystem = positionSystem;
            _eventRuleSystem = eventRuleSystem;
            _random = random;

            InitialiseBehaviours();
        }

        private void InitialiseBehaviours()
        {
            playerControlledBehaviour = new PlayerControlledBehaviour();
            randomlyMoveBehaviour = new RandomlyMoveBehaviour(_positionSystem, _eventRuleSystem, _random);
        }

        public IBehaviour Get(string behaviourName)
        {
            
            switch (behaviourName)
            {
                case "PlayerControlled":
                    return playerControlledBehaviour;
                case "RandomlyMove":
                    return randomlyMoveBehaviour;
                default:
                    throw new ArgumentException($"Could not resolve behaviour {behaviourName}");
            }
        }
    }
}
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class TryApplyTankOnAttackRule : IEventRule
    {
        private ISystemContainer _systemContainer;

        public TryApplyTankOnAttackRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public uint RuleOrder => 488;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            var defenceEventData = new DefenceEventData { ForAttack = data, DefenceType = "Tank" };

            if (_systemContainer.EventSystem.Try(EventType.Defence, data.Defender, defenceEventData))
            {
                data.SuccessfulDefenceType = "Tank";
            }

            return true;
        }
    }
}

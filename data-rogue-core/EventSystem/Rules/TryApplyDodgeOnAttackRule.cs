using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class TryApplyDodgeOnAttackRule : IEventRule
    {
        private ISystemContainer _systemContainer;

        public TryApplyDodgeOnAttackRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public int RuleOrder => 489;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            var defenceEventData = new DefenceEventData { ForAttack = data, DefenceType = "Dodge" };

            if (_systemContainer.EventSystem.Try(EventType.Defence, data.Defender, defenceEventData))
            {
                data.SuccessfulDefenceType = "Dodge";
            }

            return true;
        }
    }
}

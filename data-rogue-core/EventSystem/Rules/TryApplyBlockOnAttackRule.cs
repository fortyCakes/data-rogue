using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class TryApplyBlockOnAttackRule: IEventRule
    {
        private ISystemContainer _systemContainer;

        public TryApplyBlockOnAttackRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public int RuleOrder => 490;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            var defenceEventData = new DefenceEventData { ForAttack = data, DefenceType = "Block" };

            if (_systemContainer.EventSystem.Try(EventType.Defence, data.Defender, defenceEventData))
            {
                data.SuccessfulDefenceType = "Block";
            }

            return true;
        }
    }
}

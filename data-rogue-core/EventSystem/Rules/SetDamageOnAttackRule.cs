using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class SetDamageOnAttackRule : IEventRule
    {
        private ISystemContainer _systemContainer;

        public SetDamageOnAttackRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public int RuleOrder => 998;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            if (data.Damage == null)
            {
                data.Damage = _systemContainer.StatSystem.GetEntityStat(data.Weapon, "Damage");
            }

            return true;
        }
    }
}

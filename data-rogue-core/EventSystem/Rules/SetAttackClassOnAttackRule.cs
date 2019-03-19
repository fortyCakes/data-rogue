using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class SetAttackClassOnAttackRule : IEventRule
    {
        private ISystemContainer _systemContainer;

        public SetAttackClassOnAttackRule(ISystemContainer systemContainer)
        {
            this._systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public int RuleOrder => 999;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            if (data.AttackClass == null)
            {
                data.AttackClass = data.Weapon.Get<Weapon>().Class;
            }

            return true;
        }
    }
}

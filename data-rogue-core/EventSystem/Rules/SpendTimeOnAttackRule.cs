using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class SpendTimeOnAttackRule: IEventRule
    {
        private ISystemContainer systemContainer;

        public SpendTimeOnAttackRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public int RuleOrder => 1;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            if (data.IsAction)
            {
                systemContainer.EventSystem.Try(EventType.SpendTime, sender, new SpendTimeEventData { Ticks = data.Speed.Value });
            }

            return true;
        }
    }

}

using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class DealDamageRule : IEventRule
    {
        public DealDamageRule(ISystemContainer systemContainer)
        {
            EventRuleSystem = systemContainer.EventSystem;
            MessageSystem = systemContainer.MessageSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Damage };
        public int RuleOrder => 0;

        private IEventSystem EventRuleSystem { get; }
        public IMessageSystem MessageSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var health = sender.Get<Health>();
            var data = eventData as DamageEventData;

            health.HP.Subtract(data.Damage);

            MessageSystem.Write($"{sender.Get<Description>().Name} takes {data.Damage} damage.", Color.White);

            if (health.HP.Current <= 0)
            {
                EventRuleSystem.Try(EventType.Death, sender, new DeathEventData { Killer = data.DamagedBy });
            }

            return true;
        }
    }
}

using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class PeopleDieWhenTheyAreKilledRule : IEventRule
    {
        public PeopleDieWhenTheyAreKilledRule(ISystemContainer systemContainer)
        {
            EntityEngine = systemContainer.EntityEngine;
            MessageSystem = systemContainer.MessageSystem;
            PlayerSystem = systemContainer.PlayerSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Death };
        public uint RuleOrder => 0;

        public EventRuleType RuleType => EventRuleType.EventResolution;

        private IEntityEngine EntityEngine { get; }
        public IMessageSystem MessageSystem { get; }
        public IPlayerSystem PlayerSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (sender != PlayerSystem.Player)
            {
                MessageSystem.Write($"{sender.Get<Description>().Name} dies.", Color.DarkRed);
                EntityEngine.Destroy(sender.EntityId);
            }

            return true;
        }
    }
}

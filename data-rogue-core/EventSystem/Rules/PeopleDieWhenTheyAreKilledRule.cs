using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class PeopleDieWhenTheyAreKilledRule : IEventRule
    {
        public PeopleDieWhenTheyAreKilledRule(ISystemContainer systemContainer)
        {
            EntityEngine = systemContainer.EntityEngine;
            MessageSystem = systemContainer.MessageSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Death };
        public int RuleOrder => 0;

        private IEntityEngine EntityEngine { get; }
        public IMessageSystem MessageSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            MessageSystem.Write($"{sender.Get<Description>().Name} dies.", Color.DarkRed);
            EntityEngine.Destroy(sender.EntityId);

            return true;
        }
    }
}

using System.Drawing;
using System.Linq;
using data_rogue_core.Behaviours;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class SpendTimeRule : IEventRule
    {
        public SpendTimeRule(ISystemContainer systemContainer)
        {
            TimeSystem = systemContainer.TimeSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.SpendTime };
        public int RuleOrder => 0;

        private ITimeSystem TimeSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            Actor actor = sender.Get<Actor>();

            var spendTimeEventData = (SpendTimeEventData) eventData;

            TimeSystem.SpendTicks(sender, spendTimeEventData.Ticks);
            actor.HasActed = true;
            
            if (sender.Has<PlayerControlledBehaviour>())
            {
                TimeSystem.WaitingForInput = false;
            }

            return true;
        }
    }
}

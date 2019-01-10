using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class SpendTimeRule : IEventRule
    {
        public SpendTimeRule(ITimeSystem timeSystem)
        {
            TimeSystem = timeSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.SpendTime };
        public int RuleOrder => 0;

        private ITimeSystem TimeSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            Actor actor = sender.Get<Actor>();

            TimeSystem.SpendTicks(sender, (int)eventData);
            actor.HasActed = true;
            
            if (actor.Behaviours == "PlayerControlled")
            {
                TimeSystem.WaitingForInput = false;
            }

            return true;
        }
    }
}

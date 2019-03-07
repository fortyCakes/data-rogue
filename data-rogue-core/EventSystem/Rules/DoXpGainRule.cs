using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class DoXpGainRule : IEventRule
    {
        public EventTypeList EventTypes => new EventTypeList { EventType.GainXP };

        public int RuleOrder => 0;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as GainXPEventData;

            var experience = sender.Get<Experience>();

            experience.Amount += data.Amount;

            return true;
        }
    }
}

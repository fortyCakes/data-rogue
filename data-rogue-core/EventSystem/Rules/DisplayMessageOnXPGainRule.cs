using System.Drawing;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class DisplayMessageOnXPGainRule : IEventRule
    {
        private ISystemContainer systemContainer;

        public DisplayMessageOnXPGainRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.GainXP };

        public int RuleOrder => -1;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as GainXPEventData;

            if (systemContainer.PlayerSystem.IsPlayer(sender))
            {
                systemContainer.MessageSystem.Write($"{sender.DescriptionName} gains {data.Amount} XP.", Color.Green);
            }

            return true;
        }
    }
}

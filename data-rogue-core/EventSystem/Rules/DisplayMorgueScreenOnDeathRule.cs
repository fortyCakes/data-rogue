using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class DisplayMorgueScreenOnDeathRule : IEventRule
    {
        public DisplayMorgueScreenOnDeathRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Death };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;

        private readonly ISystemContainer systemContainer;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (systemContainer.PlayerSystem.IsPlayer(sender))
            {
                systemContainer.ActivitySystem.Push(new EndGameScreenActivity(
                    systemContainer.ActivitySystem.DefaultPosition,
                    systemContainer.ActivitySystem.DefaultPadding,
                    systemContainer, 
                    false));
            }

            return true;
        }
    }
}

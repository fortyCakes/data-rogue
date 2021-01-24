using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_one.EventSystem.Utils;

namespace data_rogue_core.EventSystem.Rules
{
    public class DisplayAdvancedMorgueScreenOnDeathRule : IEventRule
    {
        public DisplayAdvancedMorgueScreenOnDeathRule(ISystemContainer systemContainer)
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
                systemContainer.ActivitySystem.Push(new InformationActivity(systemContainer.ActivitySystem, MorgueHelper.GetStatusConfigurations(sender), sender, true, true));
            }

            return true;
        }
    }

    public class SaveMorgueFileOnDeathRule: IEventRule
    {
        public SaveMorgueFileOnDeathRule(ISystemContainer systemContainer)
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
                var morgueFileText = MorgueHelper.GenerateMorgueText(systemContainer);

                systemContainer.SaveSystem.SaveMorgueFile(morgueFileText);
            }

            return true;
        }
    }
}

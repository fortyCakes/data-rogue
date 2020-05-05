using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class LevelUpOnXPGainRule : IEventRule
    {
        public LevelUpOnXPGainRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.GainXP };

        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;

        private ISystemContainer systemContainer;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as GainXPEventData;

            if (sender.Has<Experience>())
            {
                var experience = sender.Get<Experience>();

                var nextLevel = experience.Level + 1;

                int xpForNextLevel = nextLevel * (nextLevel + 1);

                if (experience.Amount >= xpForNextLevel)
                {
                    experience.Level++;

                    systemContainer.MessageSystem.Write($"Level up! {sender.DescriptionName} is now level {experience.Level}.");

                    systemContainer.EventSystem.Try(EventType.LevelUp, sender, null);
                }
            }

            return true;
        }
    }
}

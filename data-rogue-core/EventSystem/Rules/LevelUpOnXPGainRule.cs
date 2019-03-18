using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class LevelUpOnXPGainRule: IEventRule
    {
        public LevelUpOnXPGainRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.GainXP };

        public int RuleOrder => -1;

        private ISystemContainer systemContainer;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {

            var data = eventData as GainXPEventData;

            var experience = sender.Get<Experience>();

            var nextLevel = experience.Level + 1;

            int xpForNextLevel = nextLevel * (nextLevel + 1);

            if (experience.Amount >= xpForNextLevel)
            {
                experience.Level++;

                var fighter = sender.Get<TiltFighter>();

                systemContainer.StatSystem.IncreaseStat(sender, "Agility", 2);
                systemContainer.StatSystem.IncreaseStat(sender, "Intellect", 2);
                systemContainer.StatSystem.IncreaseStat(sender, "Willpower", 2);
                systemContainer.StatSystem.IncreaseStat(sender, "Muscle", 2);

                systemContainer.MessageSystem.Write($"Level up! {sender.DescriptionName} is now level {experience.Level}.");
            }

            return true;
        }
    }
}

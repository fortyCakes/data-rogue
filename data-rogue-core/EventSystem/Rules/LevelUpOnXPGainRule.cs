using System.Drawing;
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

                var fighter = sender.Get<Fighter>();

                fighter.Agility += 2;
                fighter.Intellect += 2;
                fighter.Willpower += 2;
                fighter.Muscle += 2;

                systemContainer.MessageSystem.Write($"Level up! {sender.DescriptionName} is now level {experience.Level}.");
            }

            return true;
        }
    }
}

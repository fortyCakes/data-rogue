using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.EventSystem.Rules
{
    public class UpdateHighScoreFileOnDeathRule : IEventRule
    {
        public UpdateHighScoreFileOnDeathRule(ISystemContainer systemContainer)
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
                var score = systemContainer.EventSystem.GetStat(sender, "Score");

                systemContainer.SaveSystem.SaveHighScore(sender.DescriptionName, score);
            }

            return true;
        }
    }
}

using System;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.EventSystem.Rules
{
    public class AddStatToAccuracyRule : IEventRule
    {
        private ISystemContainer _systemContainer;

        public AddStatToAccuracyRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public int RuleOrder => 450;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            if (data.AttackRoll.HasValue)
            {
                decimal stat;
                switch (data.AttackClass)
                {
                    case "Heavy":
                        stat = _systemContainer.EventSystem.GetStat(sender, "Muscle");
                        break;
                    case "Light":
                        stat = (_systemContainer.EventSystem.GetStat(sender, "Agility") + _systemContainer.EventSystem.GetStat(sender, "Muscle")) / 2;
                        break;
                    case "Thrown":
                        stat = (_systemContainer.EventSystem.GetStat(sender, "Agility") + _systemContainer.EventSystem.GetStat(sender, "Muscle")) / 2;
                        break;
                    case "Launcher":
                        stat = _systemContainer.EventSystem.GetStat(sender, "Agility");
                        break;
                    case "Bolt":
                        stat = (_systemContainer.EventSystem.GetStat(sender, "Agility") + _systemContainer.EventSystem.GetStat(sender, "Intellect")) / 2;
                        break;
                    case "Blast":
                        stat = (_systemContainer.EventSystem.GetStat(sender, "Willpower") + _systemContainer.EventSystem.GetStat(sender, "Intellect")) / 2;
                        break;
                    default:
                        throw new Exception("Unknown AttackClass in " + this.GetType().Name);
                }

                var modifier = stat / 2 - 5;

                data.AttackRoll += (int)modifier;
            }

            return true;
        }
    }
}